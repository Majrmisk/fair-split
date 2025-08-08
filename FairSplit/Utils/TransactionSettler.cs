using FairSplit.Domain.Model;
using System.Text;

namespace FairSplit.Utils
{
    public static class TransactionSettler
    {
        public static HashSet<Payment> CalculateBestSettleOptions(Group group)
        {
            return CalculateBestSettleOptions(group, group.GetUnsettledTransactions());
        }

        public static HashSet<Payment> CalculateBestSettleOptions(Group group, List<Transaction> transactions)
        {
            Dictionary<(Member, Member), decimal> owers = [];

            foreach (var transaction in transactions)
            {
                if (transaction.IsPaidOff)
                {
                    continue;
                }
                var payer = transaction.Payer;
                var totalAmount = transaction.TotalAmount;
                foreach (var recipient in transaction.Recipients)
                {
                    var amount = recipient.Amount;
                    totalAmount -= amount;
                    if (recipient.Member == payer)
                    {
                        continue;
                    }

                    if (owers.ContainsKey((payer, recipient.Member)))
                    {
                        owers[(payer, recipient.Member)] += amount;
                    }
                    else
                    {
                        owers[(payer, recipient.Member)] = amount;
                    }
                }
                if (totalAmount > 0)
                {
                    foreach (var ower in group.GetAllMembers())
                    {
                        if (ower == payer)
                        {
                            continue;
                        }
                        if (owers.ContainsKey((payer, ower)))
                        {
                            owers[(payer, ower)] += totalAmount / group.GetAllMembers().Count;
                        }
                        else
                        {
                            owers[(payer, ower)] = totalAmount / group.GetAllMembers().Count;
                        }
                    }
                }
            }

            Graph graph = new();
            foreach (var ower in owers)
            {
                var payer = ower.Key.Item1;
                var recipient = ower.Key.Item2;
                var amount = ower.Value;

                graph.AddEdge(recipient, payer, amount);
            }

            graph.EliminateLoops();
            graph.EliminateSplits();

            return graph.ToPayments();
        }

        private class Graph
        {
            private HashSet<Node> _nodes;

            public Graph()
            {
                _nodes = [];
            }

            public HashSet<Payment> ToPayments()
            {
                var payments = new HashSet<Payment>();

                foreach (var node in _nodes)
                {
                    foreach (var edge in node.Edges)
                    {
                        var destination = edge.Key;
                        var amount = edge.Value;

                        Payment payment = new(node.Person, destination.Person, amount);
                        payments.Add(payment);
                    }
                }

                return payments;
            }

            public void AddEdge(Member source, Member destination, decimal amount)
            {
                var sourceNode = _nodes.FirstOrDefault(node => node.Person == source);
                if (sourceNode == null)
                {
                    sourceNode = new Node(source);
                    _nodes.Add(sourceNode);
                }

                var destinationNode = _nodes.FirstOrDefault(node => node.Person == destination);
                if (destinationNode == null)
                {
                    destinationNode = new Node(destination);
                    _nodes.Add(destinationNode);
                }

                sourceNode.Edges[destinationNode] = amount;
            }

            public string ToGraphviz()
            {
                var sb = new StringBuilder();
                sb.AppendLine("digraph G {");

                foreach (var node in _nodes)
                {
                    foreach (var (destination, amount) in node.Edges)
                    {
                        sb.AppendLine($"    \"{node.Person.Name}\" -> \"{destination.Person.Name}\" [label=\"{amount}\"];");
                    }
                }

                sb.AppendLine("}");
                return sb.ToString();
            }

            public void EliminateSplits()
            {
                foreach (var node in _nodes)
                {
                    RemovePredecesors();
                    while (EliminateSplitBFS(node)) ;
                }
                RemovePredecesors();
            }

            // Assumes that the graph has no loops
            private bool EliminateSplitBFS(Node start)
            {
                Queue<Node> queue = [];
                HashSet<Node> visited = [];

                queue.Enqueue(start);
                visited.Add(start);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();
                    foreach (var edge in node.Edges)
                    {
                        if (visited.Contains(edge.Key))
                        {
                            RemoveFoundSplit(edge.Key, node, edge.Value);
                            return true;
                        }
                        queue.Enqueue(edge.Key);
                        visited.Add(edge.Key);
                        edge.Key.Predecesor = (node, edge.Value);
                    }
                }
                return false;
            }

            private void RemoveFoundSplit(Node shorterRouteWithFinalNode, Node longerRoute, decimal fromLongerToFinal)
            {
                HashSet<Node> longerRouteNodes = [];

                var currentNode = longerRoute;
                while (currentNode.Predecesor != null)
                {
                    longerRouteNodes.Add(currentNode);
                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                }

                var begginerNode = shorterRouteWithFinalNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                while (!longerRouteNodes.Contains(begginerNode) && begginerNode.Predecesor != null)
                {
                    begginerNode = begginerNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                }

                var smallestValue = fromLongerToFinal;
                currentNode = longerRoute;
                while (currentNode != begginerNode)
                {
                    if (currentNode.Predecesor?.Item2 < smallestValue)
                    {
                        smallestValue = currentNode.Predecesor?.Item2 ?? throw new InvalidOperationException();
                    }
                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                }

                currentNode = longerRoute;
                var currentNodeSuccessor = shorterRouteWithFinalNode;

                do
                {
                    currentNode.Edges[currentNodeSuccessor] -= smallestValue;
                    if (currentNode.Edges[currentNodeSuccessor] == 0)
                    {
                        currentNode.Edges.Remove(currentNodeSuccessor);
                    }
                    currentNodeSuccessor = currentNode;
                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                } while (currentNode != begginerNode);
                currentNode.Edges[currentNodeSuccessor] -= smallestValue;
                if (currentNode.Edges[currentNodeSuccessor] == 0)
                {
                    currentNode.Edges.Remove(currentNodeSuccessor);
                }

                currentNodeSuccessor = shorterRouteWithFinalNode;
                currentNode = shorterRouteWithFinalNode.Predecesor?.Item1 ?? throw new InvalidOperationException();

                do
                {
                    currentNode.Edges[currentNodeSuccessor] += smallestValue;
                    if (currentNode == begginerNode)
                    {
                        return;
                    }
                    currentNodeSuccessor = currentNode;
                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                } while (currentNode != begginerNode);
            }

            public void EliminateLoops()
            {
                foreach (var node in _nodes)
                {
                    while (EliminateLoopDFS(node, []))
                    {
                        RemovePredecesors();
                    }
                }
                RemovePredecesors();
            }

            private void RemovePredecesors()
            {
                foreach (var node in _nodes)
                {
                    node.Predecesor = null;
                }
            }

            private bool EliminateLoopDFS(Node currentNode, HashSet<Node> visited)
            {
                foreach (var edge in currentNode.Edges.ToArray())
                {
                    edge.Key.Predecesor = (currentNode, edge.Value);
                    if (visited.Contains(edge.Key))
                    {
                        RemoveFoundLoop(currentNode);
                        var deez = ToGraphviz();
                        return true;
                    }
                    visited.Add(currentNode);
                    if (EliminateLoopDFS(edge.Key, visited))
                    {
                        return true;
                    }
                    edge.Key.Predecesor = null;
                    visited.Remove(currentNode);
                }

                return false;
            }

            private static void RemoveFoundLoop(Node currentNode)
            {
                var beginnerNode = currentNode;
                var smallestValue = currentNode.Predecesor?.Item2 ?? throw new InvalidOperationException();

                do
                {
                    if (currentNode.Predecesor?.Item2 < smallestValue)
                    {
                        smallestValue = currentNode.Predecesor?.Item2 ?? throw new InvalidOperationException();
                    }

                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();

                } while (currentNode != beginnerNode);

                beginnerNode = currentNode;
                var currentNodeSuccessor = currentNode;
                currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                do
                {
                    currentNode.Edges[currentNodeSuccessor] -= smallestValue;
                    if (currentNode.Edges[currentNodeSuccessor] == 0)
                    {
                        currentNode.Edges.Remove(currentNodeSuccessor);
                    }
                    currentNodeSuccessor = currentNode;
                    currentNode = currentNode.Predecesor?.Item1 ?? throw new InvalidOperationException();
                } while (currentNode != beginnerNode);
                currentNode.Edges[currentNodeSuccessor] -= smallestValue;
                if (currentNode.Edges[currentNodeSuccessor] == 0)
                {
                    currentNode.Edges.Remove(currentNodeSuccessor);
                }
            }
        }

        private class Node
        {
            public Member Person { get; }
            public Dictionary<Node, decimal> Edges { get; } = [];
            public (Node, decimal)? Predecesor { get; set; }

            public Node(Member person)
            {
                Person = person;
            }
        }
    }
}
