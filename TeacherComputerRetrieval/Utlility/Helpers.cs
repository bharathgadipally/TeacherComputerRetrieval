﻿using System;
using System.Collections.Generic;
using TeacherComputerRetrieval.DataStructures;

namespace TeacherComputerRetrieval.Utlility
{
    public static class Helpers
    {
        /// <summary>
        /// Swap two values in an IList<T> collection given their indexes.
        /// </summary>
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list.Count < 2 || firstIndex == secondIndex)   //This check is not required but Partition function may make many calls so its for perf reason
                return;

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        /// <summary>
        /// Swap two values in an ArrayList<T> collection given their indexes.
        /// </summary>
        public static void Swap<T>(this ArrayList<T> list, int firstIndex, int secondIndex)
        {
            if (list.Count < 2 || firstIndex == secondIndex)   //This check is not required but Partition function may make many calls so its for perf reason
                return;

            var temp = list[firstIndex];
            list[firstIndex] = list[secondIndex];
            list[secondIndex] = temp;
        }

        /// <summary>
        /// Populates the specified two-dimensional with a default value.
        /// </summary>
        public static void Populate<T>(this T[,] array, int rows, int columns, T defaultValue = default(T))
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    array[i, j] = defaultValue;
                }
            }
        }

    }

}