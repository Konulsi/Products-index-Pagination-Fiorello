using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int page = 1 , int take = 4) //page yeni hansi sehifededi, take yeni neche dene product gosterecek
        {
            List<Product> products = await _productService.GetPaginatedDatas(page, take); //databazada olan butun productlari gotururuk

            List<ProductListVM> mappeddatas = GetMappedDatas(products);   // elimizde olan databazadan goturduyumuz productlari birlewdririk viewmodele gonderirik viewa

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductListVM> paginatedDatas = new(mappeddatas, page, pageCount);
                                                       //mappeddata-productlarimizin listesidi(fora salib wekili falan gosterdiklerimiz)
                                                       //pageCount- ne qeder page count varsa onlardi
                                                       //page -de hal hazirda hansi sehifedeyikse odur
            return View(paginatedDatas);
        }


        private async Task<int> GetPageCountAsync(int take) //sehifedeki pagelerin sayini tapmaq uchun method
        {
            var productCount = await _productService.GetCountAsync();   //productServicenin ichindeki method vasitesile productlarin sayini elde edirik
            
            return (int)Math.Ceiling((decimal)(productCount / take)); //productlarin sayini take e(yeni her sehifede neche product olacaq) boluruk ki  neche eded sehife oldugunu tapa bilek
            // burada metodun typena gore int-e cast edirik. math.cellingin ichinde ise neticeni decimala cast edirik. chunki math decimal tipi teleb edir.
            //math.celing i ona gore ist edirikki productcount u take e boldukde qaliq qalirsa yuvarlawdirsin deye
        }
        


        private List<ProductListVM> GetMappedDatas(List<Product> products) //bir modelin ichindekileri bir bawqa  modelin ichindekilere beraberlewdirmek datalari mapp etmek adlanir
        {
            List<ProductListVM> mappedDatas = new(); // viewmodelden instans aliriq.
            // var olan databazadki productlari goturub secvirmeliyik viewmodele gondermeliyik view-a
            foreach (var product in products)  //databazadan goturduyumuz productlari fora saliriq. ichinde bir productun propertilerini viewmodelin propertilerine beraberlewdiririk
            {
                ProductListVM productVM = new()   // list<ProductListVm> den birininin prorpertilerini beraberlewdiririk databazadan gelen productun proplarina
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = product.Category.Name,
                    Count = product.Count,
                    Price = product.Price,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image
                };

                mappedDatas.Add(productVM);  //List<ProductVM e add edirik elimzde olan beraberlewdririklerimizi>
            }

            return mappedDatas;  

        }
    }
}
