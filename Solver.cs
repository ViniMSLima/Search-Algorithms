namespace MazeAI;

public class Solver
{
    public int Option { get; set; }
    public Maze Maze { get; set; } = null!;

    public string Algorithm
    {
        get
        {
            return (Option % 4) switch
            {
                0 => "DFS",
                1 => "BFS",
                2 => "dijkstra",
                _ => "aStar"
            };
        }
    }

    public void Solve()
    {
        var goal = Maze.Spaces.FirstOrDefault(s => s.Exit);

        if (Maze.Root is null || goal is null)
            return;

        Maze.Root.IsSolution = true;
        switch (Option % 4)
        {
            case 0:
                DFS(Maze.Root, goal);
                break;
            case 1:
                BFS(Maze.Root, goal);
                break;
            case 2:
                Dijkstra(Maze.Root, goal);
                break;
            case 3:
                AStar(Maze.Root, goal);
                break;
        }
    }

    private static bool DFS(Space space, Space goal)
    {
        if (space.Visited)
            return false;

        space.Visited = true;

        List<Space> Neighbours = new();
        if (space.Bottom is not null)
            Neighbours.Add(space.Bottom);
        if (space.Top is not null)
            Neighbours.Add(space.Top);
        if (space.Left is not null)
            Neighbours.Add(space.Left);
        if (space.Right is not null)
            Neighbours.Add(space.Right);

        if (EqualityComparer<Space>.Default.Equals(space, goal))
        {
            space.IsSolution = true;
            return true;
        }

        return Neighbours.Any(neighbour => !neighbour.Visited && DFS(neighbour, goal));

        // if (space.Bottom is not null)
        // {
        //     if (!space.Bottom.Visited && DFS(space.Bottom, goal))
        //     {
        //         space.Bottom.IsSolution = true;
        //         return true;
        //     }
        // }
        // if (space.Top is not null)
        // {
        //     if (!space.Top.Visited && DFS(space.Top, goal))
        //     {
        //         space.Top.IsSolution = true;
        //         return true;
        //     }
        // }
        // if (space.Left is not null)
        // {
        //     if (!space.Left.Visited && DFS(space.Left, goal))
        //     {
        //         space.Left.IsSolution = true;
        //         return true;
        //     }
        // }
        // if (space.Right is not null)
        // {
        //     if (!space.Right.Visited && DFS(space.Right, goal))
        //     {
        //         space.Right.IsSolution = true;
        //         return true;
        //     }
        // }

        // return false;
    }

    private static bool BFS(Space start, Space goal)
    {
        var queue = new Queue<Space>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var currNode = queue.Dequeue();

            if (currNode.Visited)
                continue;

            currNode.Visited = true;

            if (currNode == goal)
            {
                currNode.IsSolution = true;
                break;
            }

            if (currNode.Bottom is not null && !prev.ContainsKey(currNode.Bottom))
            {
                prev[currNode.Bottom] = currNode;
                queue.Enqueue(currNode.Bottom);
            }

            if (currNode.Top is not null && !prev.ContainsKey(currNode.Top))
            {
                prev[currNode.Top] = currNode;
                queue.Enqueue(currNode.Top);
            }

            if (currNode.Left is not null && !prev.ContainsKey(currNode.Left))
            {
                prev[currNode.Left] = currNode;
                queue.Enqueue(currNode.Left);
            }

            if (currNode.Right is not null && !prev.ContainsKey(currNode.Right))
            {
                prev[currNode.Right] = currNode;
                queue.Enqueue(currNode.Right);
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt.IsSolution = true;
            attempt = prev[attempt];
        }

        return false;
    }

    private static bool Dijkstra(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0.0f);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var currNode = queue.Dequeue();
            if (currNode == goal)
                break;
            List<Space> edges = new();

            if (currNode.Bottom is not null)
                edges.Add(currNode.Bottom);
            if (currNode.Top is not null)
                edges.Add(currNode.Top);
            if (currNode.Left is not null)
                edges.Add(currNode.Left);
            if (currNode.Right is not null)
                edges.Add(currNode.Right);

            foreach (var edge in edges)
            {
                if (edge is not null)
                {
                    edge.Visited = true;
                    var newWeight = dist[currNode] + 1;

                    if (!dist.ContainsKey(edge))
                    {
                        dist[edge] = float.PositiveInfinity;
                        prev[edge] = null!;
                    }

                    if (newWeight < dist[edge])
                    {
                        dist[edge] = newWeight;
                        prev[edge] = currNode;
                        queue.Enqueue(edge, newWeight);
                    }
                }
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt.IsSolution = true;
            attempt = prev[attempt];
        }

        return true;
    }

    private static bool AStar(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0.0f);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var currNode = queue.Dequeue();
            if (currNode == goal)
                break;

            List<Space> edges = new();

            if (currNode.Bottom is not null)
                edges.Add(currNode.Bottom);
            if (currNode.Top is not null)
                edges.Add(currNode.Top);
            if (currNode.Left is not null)
                edges.Add(currNode.Left);
            if (currNode.Right is not null)
                edges.Add(currNode.Right);

            foreach (var edge in edges)
            {
                if (edge is not null)
                {
                    edge.Visited = true;

                    float hip = (float)(Math.Pow(Math.Abs(currNode.X - goal.X), 2) + Math.Pow(Math.Abs(currNode.Y - goal.Y), 2));
                    var newWeight = dist[currNode] + hip;

                    if (!dist.ContainsKey(edge))
                    {
                        dist[edge] = float.PositiveInfinity;
                        prev[edge] = null!;
                    }

                    if (newWeight < dist[edge])
                    {
                        dist[edge] = newWeight;
                        prev[edge] = currNode;
                        queue.Enqueue(edge, newWeight);
                    }
                }
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt.IsSolution = true;
            attempt = prev[attempt];
        }

        return true;
    }
}
