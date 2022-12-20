using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SpaceUtilities
{
    public static class Utilities
    {
        static public async Task WaitUntilAsync(Func<bool> cond, int checkPeriod, CancellationToken token)
        {
            while (true)
            {
                if (cond() || token.IsCancellationRequested) break;
                await Task.Delay(checkPeriod, token);
            }
        }

        public static T[][] ToArrayArray<T>(List<List<T>> list)
        {
            T[][] res = new T[list.Count][];
            for (int i = 0; i < list.Count; i++)
            {
                res[i] = list[i].ToArray();
            }
            return res;
        }

        public static (string, string) GetReaction(IInteractiveSpaceObject receiver, IInteractiveSpaceObject other)
        {
            if (other.ReferencedList != receiver.ReferencedList) throw new System.NotImplementedException();
            InteractionList interactionList = receiver.ReferencedList;

            int sourceIndex = interactionList.InteractiveTypes.IndexOf(receiver.GetType().FullName);
            int targetIndex = interactionList.InteractiveTypes.IndexOf(other.GetType().FullName);

            return (interactionList.CalledFunc[sourceIndex, targetIndex], interactionList.CallOrder[sourceIndex, targetIndex]);
        }
    }

    [System.Serializable]
    public class Matrix<T> // For serialization of ListList
    {
        [SerializeField] List<ListWrapper<T>> myMatrix;

        #region Constructors
        public Matrix(int row, int column)
        {
            myMatrix = new List<ListWrapper<T>>(new ListWrapper<T>[row]);
            for (int i = 0; i < myMatrix.Count; i++)
            {
                myMatrix[i] = new ListWrapper<T>(column);
            }
        }
        public Matrix(int row, int column, T defaultValue) : this(row, column)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    this[i, j] = defaultValue;
                }
            }
        }

        public Matrix(int size) : this(size, size) { }
        public Matrix(int size, T defaultValue) : this(size, size, defaultValue) { } 
        #endregion


        public int Rows { get => myMatrix.Count; }
        public int Cols { get => myMatrix.First().myList.Count; }

        public T this[int x, int y]
        {
            get
            {
                return myMatrix[x][y];
            }
            set
            {
                myMatrix[x][y] = value;
            }
        }

        public void Expand(int row, int column, T defaultValue)
        {
            if (row < 0 || column < 0) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < myMatrix.Count; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    myMatrix[i].myList.Add(defaultValue);
                }
            }

            for (int i = 0; i < row; i++)
            {
                myMatrix.Add(new ListWrapper<T>(Cols));
                for (int j = 0; j < Cols; j++) myMatrix[i].myList[j] = defaultValue;
            }
        }
        public void Expand(int size, T defaultValue) => Expand(size, size, defaultValue);

        [System.Serializable]
        private class ListWrapper<R>
        {
            public List<R> myList;

            public ListWrapper(int ele)
            {
                myList = new List<R>(new R[ele]);
            }
            public R this[int key]
            {
                get
                {
                    return myList[key];
                }
                set
                {
                    myList[key] = value;
                }
            }
        }

        public T[][] ToArrays()
        {
            T[][] res = new T[myMatrix.Count][];
            for (int i = 0; i < myMatrix.Count; i++)
            {
                res[i] = myMatrix[i].myList.ToArray();
            }
            return res;
        }
    } 
}
