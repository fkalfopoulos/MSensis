using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MSensis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace MSensis.Services
{
    public class FileManager
    {

        private readonly string _imagePath;
        private readonly MSensisContext _db;
        private readonly IHostingEnvironment _hostEnviroment;

        public FileManager(IConfiguration config, MSensisContext db, IHostingEnvironment hostEnviroment)
        {
            _imagePath = config["Path:Images"]; //remember to change
            _db = db;
            _hostEnviroment = hostEnviroment;
        }

        public string GetImagePath(string PhotoId)
        {
            var photo = _db.Photos.Where(p => p.Id == PhotoId).SingleOrDefault();
            return photo.Url; 
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                string save_path = Path.Combine(Directory.GetCurrentDirectory(), _imagePath);
                string wwwRootPath = _hostEnviroment.WebRootPath;
                string mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                string fileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";
                string path = Path.Combine(wwwRootPath + "/Content/" + fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                } 

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }
        }
    }
}

 