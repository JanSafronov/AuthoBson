using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AuthoBson.Shared.Data.Models {
    public abstract class IModelTemplate<M> where M : IModelBase {
        public static bool Scheme(int[][] args) =>
            args.All(pair =>
                pair.Length == 2 && pair[0] < pair[1]
            );

        public abstract bool IsSchematic(M model);
    }
}