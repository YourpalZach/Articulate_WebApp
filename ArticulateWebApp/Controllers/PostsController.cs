using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArticulateWebApp.Data;
using ArticulateWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ArticulateWebApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // Show the post type selector first
        public IActionResult SelectType()
        {
            return View();
        }

        // For Poems
        public IActionResult CreatePoem()
        {
            ViewData["PostType"] = "Poem";
            return View("CreateTextPost");
        }

        // For Short Stories
        public IActionResult CreateStory()
        {
            ViewData["PostType"] = "Short Story";
            return View("CreateTextPost");
        }

        // For Photo Collection
        public IActionResult CreatePhoto()
        {
            return View("CreatePhotoPost");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTextPost(Post post)
        {
            Console.WriteLine("CreateTextPost() called");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model is invalid");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Error for {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                return View("CreateTextPost", post);
            }

            post.UserId = _userManager.GetUserId(User);
            post.CreatedAt = DateTime.UtcNow;

            _context.Add(post);
            await _context.SaveChangesAsync();

            Console.WriteLine("Post successfully saved!");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhotoPost(Post post, IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
            {
                ModelState.AddModelError("ImagePath", "Please select a file.");
                return View("CreatePhotoPost", post);
            }

            // Limit file size (e.g., 2MB)
            if (upload.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("ImagePath", "File must be less than 2MB.");
                return View("CreatePhotoPost", post);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            post.ImagePath = "/uploads/" + fileName;
            post.Category = "Photo Collection";
            post.UserId = _userManager.GetUserId(User);
            post.CreatedAt = DateTime.UtcNow;

            _context.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return Forbid();

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Category,ImagePath")] Post updatedPost)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) 
                return NotFound();

            if (post.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return Forbid();

            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;
            post.Category = updatedPost.Category;
            post.ImagePath = updatedPost.ImagePath;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return Forbid();

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            if (post.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
