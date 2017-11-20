using CongnitveManager;
using SPOManager;
using System;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            SPOService spoM = new SPOService();
            var image = spoM.GetPhotoInfo("Noticias", "1");
            CelebrityService cService = new CelebrityService();
            var celebriy =cService.MakeAnalysisCelebrity(image);
            Console.WriteLine(cService.GetCelebrity(celebriy));
        }
    }
}
