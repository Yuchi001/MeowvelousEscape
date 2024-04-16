using System.Collections.Generic;

namespace Managers.SavingProgress
{
    public class SaveFile
    {
        public CatSaveInstance PickedCat;
        public int CollectedCatsCount;
        public readonly Wrapper<CatSaveInstance> UnlockedCats;

        public SaveFile(SOCat defaultCat)
        {
            var defaultCatSaveInstance = new CatSaveInstance()
            {
                CatName = defaultCat.GetDisplayInfo().CatName,
                Level = 1,
            };

            PickedCat = defaultCatSaveInstance;
            CollectedCatsCount = 0;
            UnlockedCats = new Wrapper<CatSaveInstance>(new List<CatSaveInstance>
            {
                defaultCatSaveInstance
            });
        }
    }

    public class CatSaveInstance
    {
        public string CatName;
        public int Level;
    }

    public class Wrapper<T>
    {
        public readonly List<T> List;
        
        public Wrapper(List<T> list)
        {
            List = list;
        }
    }
}