using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items; // 이진트리 구조를 가지지만 배열 형식으로 저장
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];

    }

    public void Add(T _item)
    {
        _item.HeapIndex = currentItemCount;
        items[currentItemCount] = _item;
        SortUp(_item);
        currentItemCount++;
    }

    public bool Contains(T _item)
    {
        return Equals(items[_item.HeapIndex], _item);
    }

    public int Count
    {
        get { return currentItemCount; }
    }

    public void Update(T _item)
    {
        SortUp(_item);
    }

    private void SortUp(T _item)
    {
        int parentIndex = (_item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = items[parentIndex];
            if(_item.CompareTo(parentItem) > 0)
            {
                Swap(_item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (_item.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(T _item)
    {
        while(true)
        {
            int childIndexLeft = _item.HeapIndex * 2 + 1;
            int childIndexRight = _item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if(childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if(childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (_item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(_item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

        }
    }

    public T RemoveFirst()
    {
        T itemFirst = items[0]; // 맨 위 값을 제거
        currentItemCount--;
        items[0] = items[currentItemCount]; // 맨마지막에 있는 값을 맨 위로 올림
        items[0].HeapIndex = 0;
        SortDown(items[0]); // 재 정렬
        return itemFirst;
    }

    private void Swap(T _itemA, T _itemB)
    {
        items[_itemA.HeapIndex] = _itemB;
        items[_itemB.HeapIndex] = _itemA;

        int itemAIndex = _itemA.HeapIndex;
        _itemA.HeapIndex = _itemB.HeapIndex;
        _itemB.HeapIndex = itemAIndex;
    }

}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex // 배열의 인덱스 값으로 사용
    {
        get;
        set;
    }
}
