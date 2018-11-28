//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Edge_Collapse
//{
//    public abstract class AbstractHeap
//    {
//        #region internal properties
//        private int Capacity { get; set; }
//        internal int Size { get; set; }
//        internal Pair[] Nodes { get; set; }
//        #endregion

//        #region constructors
//        public AbstractHeap(int capacity)
//        {
//            Capacity = capacity;
//            Size = 0;
//            Nodes = new Pair[Capacity];
//        }
//        #endregion

//        #region helperMethods
//        public void EnlargeIfNeeded()
//        {
//            if (Size == Capacity)
//            {
//                Capacity = 2 * Capacity;
//                Array.Copy(Nodes, Nodes, Capacity);
//            }
//        }

//        public int getLeftChildIndex(int parentIndex)
//        {
//            return 2 * parentIndex + 1;
//        }

//        public bool hasLeftChild(int parentIndex)
//        {
//            return getLeftChildIndex(parentIndex) < Size;
//        }

//        public Pair leftChild(int index)
//        {
//            return Nodes[getLeftChildIndex(index)];
//        }

//        public int getRightChildIndex(int parentIndex)
//        {
//            return 2 * parentIndex + 2;
//        }

//        public bool hasRightChild(int parentIndex)
//        {
//            return getRightChildIndex(parentIndex) < Size;
//        }

//        public Pair rightChild(int index)
//        {
//            return Nodes[getRightChildIndex(index)];
//        }

//        public int getParentIndex(int childIndex)
//        {
//            return (childIndex - 1) / 2;
//        }

//        public bool hasParent(int childIndex)
//        {
//            return getParentIndex(childIndex) >= 0;
//        }

//        public Pair parent(int index)
//        {
//            return Nodes[getParentIndex(index)];
//        }

//        public void swap(int index1, int index2)
//        {
//            Pair temp = Nodes[index1];
//            Nodes[index1] = Nodes[index2];
//            Nodes[index2] = temp;
//        }

//        #endregion

//        #region available public methods

//        /// <summary>
//        /// Gets the minimum element at the root of the tree
//        /// </summary>
//        /// <returns>Int value of minimum element</returns>
//        /// <exception cref="">InvalidOperationException when heap is empty</exception>
//        public Pair peek()
//        {
//            if (Size == 0)
//                throw new InvalidOperationException("Heap is empty");

//            return Nodes[0];
//        }

//        /// <summary>
//        /// Removes the minimum element at the root of the tree
//        /// </summary>
//        /// <returns>Int value of minimum element</returns>
//        /// <exception cref="">InvalidOperationException when heap is empty</exception>
//        public Pair pop()
//        {
//            if (Size == 0)
//                throw new InvalidOperationException("Heap is empty");

//            Pair item = Nodes[0];
//            Nodes[0] = Nodes[Size - 1];
//            Size--;
//            heapifyDown();
//            return item;
//        }

//        /// <summary>
//        /// Add a new item to heap, enlarging the array if needed
//        /// </summary>
//        /// <returns>void</returns>
//        public void add(Pair item)
//        {
//            EnlargeIfNeeded();
//            Nodes[Size] = item;
//            Size++;
//            heapifyUp();
//        }
//        #endregion

//        #region abstract methods
//        internal abstract void heapifyUp();
//        internal abstract void heapifyDown();
//        #endregion
//    }
//}
