using FairSplit.Domain.Model;
using FairSplit.Utils;
using FluentAssertions;

namespace FairSplit.Tests
{
    public class SettlementTests
    {
        private static Transaction MakeTransaction(
            string name, 
            decimal total, 
            Member payer,
            IEnumerable<MemberPayment> recipients)
        {
            return new(Guid.NewGuid(), name, total, payer, DateTime.UtcNow, false, default, [.. recipients]);
        }

        private static Group MakeGroup(
            string name, 
            IEnumerable<Member> members,
            IEnumerable<Transaction> transactions)
        {
            var g = new Group(name);
            foreach (var m in members) g.AddMember(m);
            foreach (var t in transactions) g.AddTransaction(t);
            return g;
        }

        [Fact]
        public void PaidForThemselves()
        {
            var personA = new Member("PersonA");
            var personB = new Member("PersonB");

            var t1 = MakeTransaction("t1", 50m, personA,
            [
                new(personA, 50m)
            ]);
            var t2 = MakeTransaction("t2", 50m, personB,
            [
                new(personB, 50m)
            ]);

            Group group = MakeGroup("TestGroup", [personA, personB], [t1, t2]);

            var pays = TransactionSettler.CalculateBestSettleOptions(group);
            pays.Should().BeEmpty();
        }

        [Fact]
        public void TwoPeople_OneOwer()
        {
            var personA = new Member("PersonA");
            var personB = new Member("PersonB");

            var transaction = MakeTransaction("t1", 100m, personA,
            [
                new(personA, 50m),
                new(personB, 50m)
            ]);

            Group group = MakeGroup("TestGroup", [personA, personB], [transaction]);

            var pays = TransactionSettler.CalculateBestSettleOptions(group);
            pays.Should().HaveCount(1);

            var payment = pays.Single();
            payment.From.Should().Be(personB);
            payment.To.Should().Be(personA);
            payment.Amount.Should().Be(50m);
        }

        [Fact]
        public void PaidOffTransaction()
        {
            var personA = new Member("A");
            var personB = new Member("B");

            var paidOff = MakeTransaction("Done", 100m, personA,
            [
                new(personA, 50m),
                new(personB, 50m)
            ]);
            paidOff.IsPaidOff = true;

            var group = MakeGroup("TestGroup", [personA, personB], [paidOff]);

            var pays = TransactionSettler.CalculateBestSettleOptions(group);
            pays.Should().BeEmpty();
        }

        [Fact]
        public void PerfectCycle_CancelsOut()
        {
            var personA = new Member("A");
            var personB = new Member("B");
            var personC = new Member("C");

            var t1 = MakeTransaction("B paid for A", 10m, personB, 
            [
                new(personA, 10m),
            ]);
            var t2 = MakeTransaction("C paid for B", 10m, personC, 
            [
                new(personB, 10m),
            ]);
            var t3 = MakeTransaction("A paid for C", 10m, personA, 
            [
                new(personC, 10m),
            ]);
            var group = MakeGroup("TestGroup", [personA, personB, personC], [t1, t2, t3]);

            var pays = TransactionSettler.CalculateBestSettleOptions(group);
            pays.Should().BeEmpty();
        }

        [Fact]
        public void FourPeople_TransitivePayment()
        {
            var a = new Member("A");
            var b = new Member("B");
            var c = new Member("C");
            var d = new Member("D");

            var t1 = MakeTransaction("B for A", 10m, b,
            [
                new MemberPayment(a, 10m),
            ]);
            var t2 = MakeTransaction("C for B", 10m, c,
            [
                new MemberPayment(b, 10m),
            ]);
            var t3 = MakeTransaction("D for C", 10m, d,
            [
                new MemberPayment(c, 10m),
            ]);

            var group = MakeGroup("TestGroup", [a, b, c, d], [t1, t2, t3]);
            var pays = TransactionSettler.CalculateBestSettleOptions(group);

            pays.Should().HaveCount(1);
            var p = pays.Single();
            p.From.Should().Be(a);
            p.To.Should().Be(d);
            p.Amount.Should().Be(10m);
        }

        [Fact]
        public void ThreePeople_SharedPayment()
        {
            var personA = new Member("PersonA");
            var personB = new Member("PersonB");
            var personC = new Member("PersonC");

            var t1 = MakeTransaction("t1", 90m, personA, []);
            var group = MakeGroup("TestGroup", [personA, personB, personC], [t1]);

            var pays = TransactionSettler.CalculateBestSettleOptions(group);

            pays.Should().HaveCount(2);
            pays.Should().ContainEquivalentOf(new Payment(personB, personA, 30m));
            pays.Should().ContainEquivalentOf(new Payment(personC, personA, 30m));
        }
    }
}
