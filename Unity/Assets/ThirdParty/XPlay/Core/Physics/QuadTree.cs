using System;
using System.Collections.Generic;

namespace QuadTree
{
    /// <summary>
    /// A quad tree where leaf nodes contain a quad and a unique instance of T.
    /// For example, if you are developing a game, you might use QuadTree<GameObject>
    /// for collisions, or QuadTree<int> if you just want to populate it with IDs.
    /// </summary>
    public class QuadTree<T>
    {
        internal static Stack<Branch> branchPool = new Stack<Branch>();
        internal static Stack<Leaf> leafPool = new Stack<Leaf>();

        Branch root;
        internal int splitCount;
		internal int depthLimit;
        internal Dictionary<T, Leaf> leafLookup = new Dictionary<T, Leaf>();

        /// <summary>
        /// Creates a new QuadTree.
        /// </summary>
        /// <param name="splitCount">How many leaves a branch can hold before it splits into sub-branches.</param>
		/// <param name="depthLimit">Maximum distance a node can be from the tree root.</param>
        /// <param name="region">The region that your quadtree occupies, all inserted quads should fit into this.</param>
        public QuadTree(int splitCount, int depthLimit, ref Quad region)
        {
            this.splitCount = splitCount;
			this.depthLimit = depthLimit;
            root = CreateBranch(this, null, 0, ref region);
        }
        /// <summary>
        /// Creates a new QuadTree.
        /// </summary>
        /// <param name="splitCount">How many leaves a branch can hold before it splits into sub-branches.</param>
		/// <param name="depthLimit">Maximum distance a node can be from the tree root.</param>
        /// <param name="region">The region that your quadtree occupies, all inserted quads should fit into this.</param>
		public QuadTree(int splitCount, int depthLimit, Quad region)
            : this(splitCount, depthLimit, ref region)
        {

        }
        /// <summary>
        /// Creates a new QuadTree.
        /// </summary>
        /// <param name="splitCount">How many leaves a branch can hold before it splits into sub-branches.</param>
		/// <param name="depthLimit">Maximum distance a node can be from the tree root.</param>
        /// <param name="x">X position of the region.</param>
        /// <param name="y">Y position of the region.</param>
        /// <param name="width">Width of the region.</param>
        /// <param name="height">Height of the region.</param>
        public QuadTree(int splitCount, int depthLimit, float x, float y, float width, float height)
            : this(splitCount, depthLimit, new Quad(x, y, x + width, y + height))
        {

        }

        /// <summary>
        /// Clear the QuadTree. This will remove all leaves and branches. If you have a lot of moving objects,
        /// you probably want to call Clear() every frame, and re-insert every object. Branches and leaves are pooled.
        /// </summary>
        public void Clear()
        {
            root.Clear();
            root.Tree = this;
            leafLookup.Clear();
        }

        /// <summary>
        /// QuadTree internally keeps pools of Branches and Leaves. If you want to clear these to clean up memory,
        /// you can call this function. Most of the time you'll want to leave this alone, though.
        /// </summary>
        public static void ClearPools()
        {
            branchPool = new Stack<Branch>();
            leafPool = new Stack<Leaf>();
        }

        /// <summary>
        /// Insert a new leaf node into the QuadTree.
        /// </summary>
        /// <param name="value">The leaf value.</param>
        /// <param name="quad">The leaf size.</param>
        public void Insert(T value, ref Quad quad)
        {
            Leaf leaf;
            if (!leafLookup.TryGetValue(value, out leaf))
            {
                leaf = CreateLeaf(value, ref quad);
                leafLookup.Add(value, leaf);
            }
            root.Insert(leaf);
        }
        /// <summary>
        /// Insert a new leaf node into the QuadTree.
        /// </summary>
        /// <param name="value">The leaf value.</param>
        /// <param name="quad">The leaf quad.</param>
        public void Insert(T value, Quad quad)
        {
            Insert(value, ref quad);
        }
        /// <summary>
        /// Insert a new leaf node into the QuadTree.
        /// </summary>
        /// <param name="value">The leaf value.</param>
        /// <param name="x">X position of the leaf.</param>
        /// <param name="y">Y position of the leaf.</param>
        /// <param name="width">Width of the leaf.</param>
        /// <param name="height">Height of the leaf.</param>
        public void Insert(T value, float x, float y, float width, float height)
        {
            var quad = new Quad(x, y, x + width, y + height);
            Insert(value, ref quad);
        }

        /// <summary>
        /// Find all values contained in the specified area.
        /// </summary>
        /// <returns>True if any values were found.</returns>
        /// <param name="quad">The area to search.</param>
        /// <param name="values">A list to populate with the results. If null, this function will create the list for you.</param>
        public bool SearchArea(ref Quad quad, ref List<T> values)
        {
            if (values != null)
                values.Clear();
            else
                values = new List<T>();
            root.SearchQuad(ref quad, values);
            return values.Count > 0;
        }
        /// <summary>
        /// Find all values contained in the specified area.
        /// </summary>
        /// <returns>True if any values were found.</returns>
        /// <param name="quad">The area to search.</param>
        /// <param name="values">A list to populate with the results. If null, this function will create the list for you.</param>
        public bool SearchArea(Quad quad, ref List<T> values)
        {
            return SearchArea(ref quad, ref values);
        }
        /// <summary>
        /// Find all values contained in the specified area.
        /// </summary>
        /// <returns>True if any values were found.</returns>
        /// <param name="x">X position to search.</param>
        /// <param name="y">Y position to search.</param>
        /// <param name="width">Width of the search area.</param>
        /// <param name="height">Height of the search area.</param>
        /// <param name="values">A list to populate with the results. If null, this function will create the list for you.</param>
        public bool SearchArea(float x, float y, float width, float height, ref List<T> values)
        {
            var quad = new Quad(x, y, x + width, y + height);
            return SearchArea(ref quad, ref values);
        }

        /// <summary>
        /// Find all values overlapping the specified point.
        /// </summary>
        /// <returns>True if any values were found.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="values">A list to populate with the results. If null, this function will create the list for you.</param>
        public bool SearchPoint(float x, float y, ref List<T> values)
        {
            if (values != null)
                values.Clear();
            else
                values = new List<T>();
            root.SearchPoint(x, y, values);
            return values.Count > 0;
        }

        /// <summary>
        /// Find all other values whose areas are overlapping the specified value.
        /// </summary>
        /// <returns>True if any collisions were found.</returns>
        /// <param name="value">The value to check collisions against.</param>
        /// <param name="values">A list to populate with the results. If null, this function will create the list for you.</param>
        public bool FindCollisions(T value, ref List<T> values)
        {
            if (values != null)
                values.Clear();
            else
                values = new List<T>(leafLookup.Count);

            Leaf leaf;
            if (leafLookup.TryGetValue(value, out leaf))
            {
                var branch = leaf.Branch;

                //Add the leaf's siblings (prevent it from colliding with itself)
                if (branch.Leaves.Count > 0)
                    for (int i = 0; i < branch.Leaves.Count; ++i)
                        if (leaf != branch.Leaves[i] && leaf.Quad.Intersects(ref branch.Leaves[i].Quad))
                            values.Add(branch.Leaves[i].Value);

                //Add the branch's children
                if (branch.Split)
                    for (int i = 0; i < 4; ++i)
                        if (branch.Branches[i] != null)
                            branch.Branches[i].SearchQuad(ref leaf.Quad, values);

                //Add all leaves back to the root
                branch = branch.Parent;
                while (branch != null)
                {
                    if (branch.Leaves.Count > 0)
                        for (int i = 0; i < branch.Leaves.Count; ++i)
                            if (leaf.Quad.Intersects(ref branch.Leaves[i].Quad))
                                values.Add(branch.Leaves[i].Value);
                    branch = branch.Parent;
                }
            }
            return false;
        }

        /// <summary>
        /// Count how many branches are in the QuadTree.
        /// </summary>
        public int CountBranches()
        {
            int count = 0;
            CountBranches(root, ref count);
            return count;
        }
        void CountBranches(Branch branch, ref int count)
        {
            ++count;
            if (branch.Split)
                for (int i = 0; i < 4; ++i)
                    if (branch.Branches[i] != null)
                        CountBranches(branch.Branches[i], ref count);
        }

        static Branch CreateBranch(QuadTree<T> tree, Branch parent, int branchDepth, ref Quad quad)
        {
            var branch = branchPool.Count > 0 ? branchPool.Pop() : new Branch();
            branch.Tree = tree;
            branch.Parent = parent;
            branch.Split = false;
			branch.Depth = branchDepth;
            float midX = quad.MinX + (quad.MaxX - quad.MinX) * 0.5f;
            float midY = quad.MinY + (quad.MaxY - quad.MinY) * 0.5f;
            branch.Quads[0].Set(quad.MinX, quad.MinY, midX, midY);
            branch.Quads[1].Set(midX, quad.MinY, quad.MaxX, midY);
            branch.Quads[2].Set(midX, midY, quad.MaxX, quad.MaxY);
            branch.Quads[3].Set(quad.MinX, midY, midX, quad.MaxY);
            return branch;
        }

        static Leaf CreateLeaf(T value, ref Quad quad)
        {
            var leaf = leafPool.Count > 0 ? leafPool.Pop() : new Leaf();
            leaf.Value = value;
            leaf.Quad = quad;
            return leaf;
        }

        internal class Branch
        {
            internal QuadTree<T> Tree;
            internal Branch Parent;
            internal Quad[] Quads = new Quad[4];
            internal Branch[] Branches = new Branch[4];
            internal List<Leaf> Leaves = new List<Leaf>();
            internal bool Split;
			internal int Depth;

            internal void Clear()
            {
                Tree = null;
                Parent = null;
                Split = false;

                for (int i = 0; i < 4; ++i)
                {
                    if (Branches[i] != null)
                    {
                        branchPool.Push(Branches[i]);
                        Branches[i].Clear();
                        Branches[i] = null;
                    }
                }

                for (int i = 0; i < Leaves.Count; ++i)
                {
                    leafPool.Push(Leaves[i]);
                    Leaves[i].Branch = null;
                    Leaves[i].Value = default(T);
                }

                Leaves.Clear();
            }

            internal void Insert(Leaf leaf)
            {
                //If this branch is already split
                if (Split)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        if (Quads[i].Contains(ref leaf.Quad))
                        {
                            if (Branches[i] == null)
                                Branches[i] = CreateBranch(Tree, this, Depth+1, ref Quads[i]);
                            Branches[i].Insert(leaf);
                            return;
                        }
                    }

                    Leaves.Add(leaf);
                    leaf.Branch = this;
                }
                else
                {
                    //Add the leaf to this node
                    Leaves.Add(leaf);
                    leaf.Branch = this;

                    //Once I have reached capacity, split the node
                    if (Leaves.Count >= Tree.splitCount && Depth < Tree.depthLimit)
                    {
                        Split = true;
                    }
                }
            }

            internal void SearchQuad(ref Quad quad, List<T> values)
            {
                if (Leaves.Count > 0)
                    for (int i = 0; i < Leaves.Count; ++i)
                        if (quad.Intersects(ref Leaves[i].Quad))
                            values.Add(Leaves[i].Value);
                for (int i = 0; i < 4; ++i)
                    if (Branches[i] != null)
                        Branches[i].SearchQuad(ref quad, values);
            }

            internal void SearchPoint(float x, float y, List<T> values)
            {
                if (Leaves.Count > 0)
                    for (int i = 0; i < Leaves.Count; ++i)
                        if (Leaves[i].Quad.Contains(x, y))
                            values.Add(Leaves[i].Value);
                for (int i = 0; i < 4; ++i)
                    if (Branches[i] != null)
                        Branches[i].SearchPoint(x, y, values);
            }
        }

        internal class Leaf
        {
            internal Branch Branch;
            internal T Value;
            internal Quad Quad;
        }
    }

    /// <summary>
    /// Used by the QuadTree to represent a rectangular area.
    /// </summary>
    public struct Quad
    {
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;

        /// <summary>
        /// Construct a new Quad.
        /// </summary>
        /// <param name="minX">Minimum x.</param>
        /// <param name="minY">Minimum y.</param>
        /// <param name="maxX">Max x.</param>
        /// <param name="maxY">Max y.</param>
        public Quad(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        /// <summary>
        /// Set the Quad's position.
        /// </summary>
        /// <param name="minX">Minimum x.</param>
        /// <param name="minY">Minimum y.</param>
        /// <param name="maxX">Max x.</param>
        /// <param name="maxY">Max y.</param>
        public void Set(float minX, float minY, float maxX, float maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        /// <summary>
        /// Check if this Quad intersects with another.
        /// </summary>
        public bool Intersects(ref Quad other)
        {
            return MinX < other.MaxX && MinY < other.MaxY && MaxX > other.MinX && MaxY > other.MinY;
        }

        /// <summary>
        /// Check if this Quad can completely contain another.
        /// </summary>
        public bool Contains(ref Quad other)
        {
            return other.MinX >= MinX && other.MinY >= MinY && other.MaxX <= MaxX && other.MaxY <= MaxY;
        }

        /// <summary>
        /// Check if this Quad contains the point.
        /// </summary>
        public bool Contains(float x, float y)
        {
            return x > MinX && y > MinY && x < MaxX && y < MaxY;
        }
    }
}

