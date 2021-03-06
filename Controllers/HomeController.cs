﻿using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Filters;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    [LoginRequiredFilter]
    public class HomeController : Controller
    {
        private readonly IPostsRepo _postsRepo;
        public HomeController()
        {
            _postsRepo = DalFactory.GetPostsRepo();
        }

        public HomeController(IPostsRepo postsRepo)
        {
            _postsRepo = postsRepo;
        }

        public ViewResult Index()
        {
            ViewBag.VisibilityOptions = _postsRepo.GetVisibilityOptions();
            return View();
        }
    }
}