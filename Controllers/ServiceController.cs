using ClosedXML.Excel;
using Cricket_Club.Models;
using Cricket_Club.Models.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Cricket_Club.Controllers
{
    public class ServiceController : Controller
    {
        private readonly DataContext datacontext;

        public ServiceController(DataContext context)
        {
            datacontext = context;
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult NewMember()
        {
            return View();
        }
        public IActionResult ViewMember()
        {
            var data = datacontext.cricketer.ToList();
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> NewMember(Cricketer cre)
        {
            var data = new Cricketer()
            {
                Id = Guid.NewGuid(),
                Name = cre.Name,
                Gender = cre.Gender,
                Team = cre.Team,
                MatchPlayed = cre.MatchPlayed,
                TotalScore = cre.TotalScore,
                Salary = cre.Salary,
            };
            await datacontext.cricketer.AddAsync(data);
            await datacontext.SaveChangesAsync();
            return RedirectToAction("NewMember");
        }

        public IActionResult EditMember(Guid Id)
        {
            var data = datacontext.cricketer.FirstOrDefault(x => x.Id == Id);
            if (data != null)
            {
                var view = new Cricketer()
                {
                    Id = data.Id,
                    Name = data.Name,
                    Gender = data.Gender,
                    Team = data.Team,
                    MatchPlayed = data.MatchPlayed,
                    TotalScore = data.TotalScore,
                    Salary = data.Salary,
                };
                return View(view);
            }
            return RedirectToAction("ViewMember");
        }

        [HttpPost]
        public IActionResult EditMember(Cricketer model)
        {
            var data = datacontext.cricketer.FirstOrDefault(x => x.Id == model.Id);
            if (data != null)
            {

                data.Id = model.Id;
                data.Name = model.Name;
                data.Gender = model.Gender;
                data.Team = model.Team;
                data.MatchPlayed = model.MatchPlayed;
                data.TotalScore = model.TotalScore; 
                data.Salary = model.Salary;
                datacontext.cricketer.Update(data);
                datacontext.SaveChangesAsync();
                return View(model);
            }
            return RedirectToAction("ViewMember");
        }

        [HttpPost]
        public IActionResult DeleteMember(Cricketer update)
        {
            var Id = update.Id;
            var data = datacontext.cricketer.FirstOrDefault(x => x.Id == Id);
            if (data != null)
            {
                datacontext.cricketer.Remove(data);
                datacontext.SaveChangesAsync();

            }
            return RedirectToAction("ViewMember");
        }

        public IActionResult ImportViaExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportViaExcel(ImportVM model)
        {
            using (var stream = new MemoryStream())
            {
                await model.file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        string matchplayed = worksheet.Cells[row, 5].Value.ToString().Trim();
                        string totalscore = worksheet.Cells[row, 6].Value.ToString().Trim();
                        string salary = worksheet.Cells[row, 7].Value.ToString().Trim();

                        var data = new Cricketer()
                        {
                            Id = Guid.NewGuid(),
                            Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Gender = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            Team = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            MatchPlayed = Int32.Parse(matchplayed),
                            TotalScore = Int32.Parse(totalscore),
                            Salary = Int32.Parse(salary),
                        };
                        await datacontext.cricketer.AddAsync(data);
                        await datacontext.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("ViewMember");
        }
        public IActionResult ExportViaExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("sheet1");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Gender";
                worksheet.Cell(currentRow, 4).Value = "Team";
                worksheet.Cell(currentRow, 5).Value = "MatchPlayed";
                worksheet.Cell(currentRow, 6).Value = "TotalScore";
                worksheet.Cell(currentRow, 7).Value = "Salary";
                foreach (var user in datacontext.cricketer)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Id.ToString();
                    worksheet.Cell(currentRow, 2).Value = user.Name;
                    worksheet.Cell(currentRow, 3).Value = user.Gender;
                    worksheet.Cell(currentRow, 4).Value = user.Team;
                    worksheet.Cell(currentRow, 5).Value = user.MatchPlayed;
                    worksheet.Cell(currentRow, 6).Value = user.TotalScore;
                    worksheet.Cell(currentRow, 7).Value = user.Salary;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Cricket_Club.xlsx");
                }
            }
        }
    }
}
