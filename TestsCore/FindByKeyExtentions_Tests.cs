
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EntityFrameworkCore.PrimaryKey.TestsCore
{
 public    class SingleColumnFind:TestBase
    {
        A e = new A { Id = 42L };
        A e1 = new A { Id = 43L };
        [Fact]
        public void FindTest()
        {
            var source = new List<A> { e, e1 }.AsQueryable();
            var key =e.GetPrimaryKey();
            var finded = source.Find(key);
            Assert.Equal(e, finded);
        }
    }
}
