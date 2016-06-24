using System.Linq;
using EFDal.Context;
using EFDal.Dal;
using EFDal.Models;

namespace MHD.Original.Award.Services
{
    public class UserService
    {
        private Original_comic_libraryContext context = new Original_comic_libraryContext(ContextType.originalRead);

        public original_author_info getAuthor(string authoremail, string authorpwd)
        {
            return context.original_author_info.FirstOrDefault(x => x.email == authoremail && x.password == authorpwd && !x.isdelete && !x.ispause);
        }

        public original_author_info getAuthorById(int authorid)
        {
            return context.original_author_info.FirstOrDefault(x => x.id == authorid);
        }

        public original_author_info getAuthorByEmail(string email)
        {
            return context.original_author_info.FirstOrDefault(x => x.email == email);
        }

        public original_author_info insertauthor_info(original_author_info record)
        {
            using (Original_comic_libraryContext context = new Original_comic_libraryContext(ContextType.originalCon))
            {
                record = context.original_author_info.Add(record);
                context.SaveChanges();
            }
            return record;
        }

        public int deleteauthor_info(int id)
        {
            using (Original_comic_libraryContext context = new Original_comic_libraryContext(ContextType.originalCon))
            {
                var author_info = context.original_author_info.FirstOrDefault(x => x.id == id);
                if (author_info != null)
                {
                    context.original_author_info.Remove(author_info);

                    if (context.SaveChanges() > 0)
                    {
                        return id;
                    }
                }
            }
            return 0;
        }
    }
}