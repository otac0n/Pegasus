// -----------------------------------------------------------------------
// <copyright file="Identifier.cs" company="(none)">
//   Copyright © 2012 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.txt for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Expressions
{
    using System;
    using Pegasus.Parser;

    public class Identifier
    {
        private readonly Cursor end;
        private readonly string name;
        private readonly Cursor start;

        public Identifier(string name, Cursor start, Cursor end)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
            this.start = start;
            this.end = end;
        }

        public Cursor End
        {
            get { return this.end; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public Cursor Start
        {
            get { return this.start; }
        }
    }
}
