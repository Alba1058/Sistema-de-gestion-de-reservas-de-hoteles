using Microsoft.EntityFrameworkCore; 
using SGHR.Persistence.Context;

namespace SGHR.Persistence.Test2.Context
{
    public class SGHRContextTest
    {
        private readonly SGHRContext _context;

        protected SGHRContextTest()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
        }
    }
}