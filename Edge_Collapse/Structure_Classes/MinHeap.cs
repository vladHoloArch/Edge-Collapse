//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace VertexDecimation
//{
//    public class MinHeap : AbstractHeap
//    {
//        #region constructors
//        public MinHeap(int capacity) : base(capacity)
//        {
//        }
//        #endregion

//        #region internal methods
//        internal override void heapifyDown()
//        {
//            int index = 0;
//            while (hasLeftChild(index))
//            {
//                int smallerChildIndex = getLeftChildIndex(index);
//                if (hasRightChild(index) && rightChild(index).one.CombinedQerrorSize < leftChild(index).one.CombinedQerrorSize)
//                {
//                    smallerChildIndex = getRightChildIndex(index);
//                }

//                if (Nodes[smallerChildIndex].one.CombinedQerrorSize < Nodes[index].one.CombinedQerrorSize)
//                    swap(index, smallerChildIndex);
//                else
//                    break;
//                index = smallerChildIndex;
//            }
//        }
//        internal override void heapifyUp()
//        {
//            int index = Size - 1;

//            while (hasParent(index) && parent(index).one.CombinedQerrorSize > Nodes[index].one.CombinedQerrorSize)
//            {
//                swap(index, getParentIndex(index));
//                index = getParentIndex(index);
//            }
//        }
//        #endregion
//    }
//}
