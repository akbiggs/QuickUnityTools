using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Asserts {
    public static void NotNull(object obj) {
        if (obj == null) {
            throw new NullReferenceException("This variable should be never be null!");
        }
    }
}
