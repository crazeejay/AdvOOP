using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generics
{

    public class CustomList<T>
    {

        public int amount;
        private T[] data;


        public CustomList() { amount = 0; }

        public void Add(T item)
        {
            //Create a new array of amount + 1
            T[] cache = new T[amount + 1];
            //Check if the list has been initialized 
            if(data != null)
            {
              //Copy all existing items to new array
              for (int i = 0; i < data.Length; i++)
              {
                    cache[i] = data[i];

              }

            }
            //Set the end element to new item
            cache[amount] = item;
            //point to new array
            data = cache;
            //Increment amount
            amount++;
        }

    }
}
