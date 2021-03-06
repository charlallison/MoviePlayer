﻿
using Movies.Models.Model;
using Movies.MoviesInterfaces;
using Movies.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoComponent.BaseClass
{
    public class SortByDate : IComparer<VideoFolder>
    {
        public int Compare(VideoFolder x, VideoFolder y)
        {
            if (x == null || y == null)
                return 0;
            if (x.FileType == y.FileType)
            {
                var vv = x.FileInfo.CreationTime.CompareTo(y.FileInfo.CreationTime);
                return vv * -1;
            }
            else if (x.FileType == FileType.Folder)
            {
                return -1;
            }
            return 1;
        }
    }
}
