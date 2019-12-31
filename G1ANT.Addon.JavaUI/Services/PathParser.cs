﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace G1ANT.Addon.JavaUI.Services
{
    public class PathParser : IPathParser
    {
        public const char PathSeparator = '/';
        public const char Wildcard = '*';

        public List<PathElement> Parse(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var elementsPath = path
                .Split(new char[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(pe => new PathElement(pe))
                .ToList();

            if (elementsPath.Count() < 2)
                throw new ArgumentException("Please specify path that contains at least JVM id and window handle");

            if (elementsPath[0].Id == 0 && !elementsPath[0].IsWildcard)
                throw new ArgumentException("First element of path must be JVM id or a wildcard (*)");

            return elementsPath;
        }

    }
}