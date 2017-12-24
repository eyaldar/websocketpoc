﻿using System;
using System.Collections.Generic;

namespace DeepStreamNet
{
    class RemoteProcedureEqualityComparer : IEqualityComparer<RemoteProcedure>
    {
        public bool Equals(RemoteProcedure x, RemoteProcedure y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(RemoteProcedure obj) => obj.Name.GetHashCode();
    }
}
