using JokesMVC2023.Areas.Identity.Data;
using JokesMVC2023.Models;
using JokesMVC2023.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JokesMVC2023.Controllers
{
    public class JokeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JokeDBContext _jokeContext;
        private readonly ProfilePictureUploader _profilePictureUploader;

        public JokeController(JokeDBContext jokeContext, UserManager<AppUser> userManager, ProfilePictureUploader profilePictureUploader)
        {
            _profilePictureUploader = profilePictureUploader;
            _userManager = userManager;
            _jokeContext = jokeContext;
        }

        [AllowAnonymous]
        // GET: JokeController
        public ActionResult Index()
        {
            var jokes = _jokeContext.Jokes.Include(c => c.AppUser).AsEnumerable();

            return View("PublicJokes", jokes);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Search(IFormCollection formCollection)
        {
            var jokes = _jokeContext.Jokes.AsQueryable();

            var question = formCollection["questionSearch"].ToString();
            var answer = formCollection["answerSearch"].ToString();

            if (!String.IsNullOrEmpty(question))
            {
                jokes = jokes.Where(c => c.JokeQuestion.Contains(question));
            }

            if (!String.IsNullOrEmpty(answer))
            {
                jokes = jokes.Where(c => c.JokeAnswer.Contains(answer));
            }

            var jokesResult = jokes.ToList();
            return View("PublicJokes", jokesResult);

        }

        [AllowAnonymous]
        // GET: JokeController/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var joke = _jokeContext.Jokes.FirstOrDefault(c => c.Id == id);

            return joke != null ? View(joke) : RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: JokeController/Create
        public ActionResult Create(string id)
        {
            var joke = new JokeCreateDTO();
            joke.UserId = id;
            return View(joke);
        }

        [Authorize]
        // POST: JokeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JokeCreateDTO jokeCreate)
        {
            try
            {
                // simple error handling
                if (ModelState.IsValid)
                {
                    _jokeContext.Jokes.Add(new Joke { JokeQuestion = jokeCreate.JokeQuestion, JokeAnswer = jokeCreate.JokeAnswer, AppUserId = jokeCreate.UserId });
                    _jokeContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        // GET: JokeController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var joke = _jokeContext.Jokes.Include(j => j.AppUser).FirstOrDefault(c => c.Id == id);
            if (VerfyUserAgaistjoke(joke)) { return joke != null ? View(joke) : RedirectToAction(nameof(Index)); }
            return BadRequest();
        }

        [Authorize]
        // POST: JokeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Joke joke)
        {
            if (id != joke.Id)
            {
                return NotFound();
            }

            if (VerfyUserAgaistjoke(joke))
            {
                if (ModelState.IsValid)
                {
                    _jokeContext.Jokes.Update(joke);

                    return RedirectToAction(nameof(Index));
                }
                return View(joke);
            }
            return BadRequest();
        }

        [Authorize]
        // GET: JokeController/Delete/5
        public ActionResult Delete(int id)
        {

            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var joke = _jokeContext.Jokes.Where(c => c.Id == id).Include(j => j.AppUser).FirstOrDefault();

            if (joke == null)
            {
                return BadRequest();
            }

            if (VerfyUserAgaistjoke(joke))
            {
                return View(joke);
            }

            return BadRequest();
        }

        [Authorize]
        // POST: JokeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var joke = _jokeContext.Jokes.Include(j => j.AppUser).FirstOrDefault(c => c.Id == id);
                if (VerfyUserAgaistjoke(joke))
                {
                    if (joke != null)
                    {
                        _jokeContext.Jokes.Remove(joke);
                        _jokeContext.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                    return View();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return View();
            }
        }

        private bool VerfyUserAgaistjoke(Joke? joke)
        {
            return joke.AppUser.Id == _userManager.GetUserId(User);
        }

        public async void LoadProfilePhoto(string fileName)
        {
            byte[] fileBytes = await _profilePictureUploader.ReadFileIntoMemory(fileName);
            var imageDate = Convert.ToBase64String(fileBytes);
            var fileExtention = _profilePictureUploader.GetFileExtentsion(fileName);


            ViewData["ImageSource"] = $"data:image/{fileExtention};base64,{imageDate}";
            ViewData["ImaageAlt"] = "Image Loaded";


        }
    }
}
