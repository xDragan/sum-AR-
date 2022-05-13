using System.Collections.Generic;
using UnityEngine;

/*
 * Funciones de utilidad 
 */
public class Utils
{

    /*
     * Devuelve una la lista de un tamaño determinado de enteros dentro del rango pasado en orden aleatorio
     */
    public static List<int> RandomListOfInt(int start, int end, int sizeList)
    {
        List<int> initialList = new List<int>();
        int counter = 0;
        bool follow = true;
        while (follow)
        {
            for (int i = start; i < end; i++)
            {
                initialList.Add(i);
                counter++;
            }
            if (counter > sizeList)
            {
                follow = false;
            }
        }
        return Shuffle<int>(initialList, sizeList);
    }

    /*
     * Devuelve una la lista de un tamaño determinado de objetos de una lista en orden aleatorio
     */
    public static List<T> Shuffle<T>(List<T> orderedList, int sizeList)
    {
        List<T> response = new List<T>();
        int size = orderedList.Count;
        if (sizeList > size)
        {
            sizeList = size;
        }
        int index = 0;
        int random = 0;
        int[] temporalList = new int[size];
        for (int i = 0; i < size; i++)
        {
            temporalList[i] = i;
        }
        while (index < size)
        {
            random = Random.Range(index, size);
            int temporal = temporalList[index];
            temporalList[index] = temporalList[random];
            temporalList[random] = temporal;
            index++;
        }
        for (int i = 0; i < sizeList; i++)
        {
            response.Add(orderedList[temporalList[i]]);
        }
        return response;
    }
}
