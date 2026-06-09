using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LaboratoryExperimentsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Пути к файлам
            string dbPath = "experiments.db";
            string labCsv = Path.Combine(AppContext.BaseDirectory, "Data", "laboratories.csv");
            string expCsv = Path.Combine(AppContext.BaseDirectory, "Data", "experiments.csv");

            // Создаём менеджер БД и инициализируем данные
            var db = new DatabaseManager(dbPath);

            // Проверяем, существуют ли CSV-файлы
            if (!File.Exists(labCsv))
            {
                Console.WriteLine($"ОШИБКА: Файл не найден: {labCsv}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }
            if (!File.Exists(expCsv))
            {
                Console.WriteLine($"ОШИБКА: Файл не найден: {expCsv}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            db.InitializeDatabase(labCsv, expCsv);
            Console.WriteLine();

            // Главный цикл меню
            string choice;
            do
            {
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║      УПРАВЛЕНИЕ ЭКСПЕРИМЕНТАМИ       ║");
                Console.WriteLine("╠══════════════════════════════════════╣");
                Console.WriteLine("║ 1 — Показать все лаборатории         ║");
                Console.WriteLine("║ 2 — Показать все эксперименты        ║");
                Console.WriteLine("║ 3 — Добавить эксперимент             ║");
                Console.WriteLine("║ 4 — Редактировать эксперимент        ║");
                Console.WriteLine("║ 5 — Удалить эксперимент              ║");
                Console.WriteLine("║ 6 — Отчёты                           ║");
                Console.WriteLine("║ 0 — Выход                            ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.Write("Ваш выбор: ");

                choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowLaboratories(db); break;
                    case "2": ShowExperiments(db); break;
                    case "3": AddExperiment(db); break;
                    case "4": EditExperiment(db); break;
                    case "5": DeleteExperiment(db); break;
                    case "6": ReportsMenu(db); break;
                    case "0": Console.WriteLine("До свидания!"); break;
                    default: Console.WriteLine("Неверный пункт меню."); break;
                }
                Console.WriteLine();
            } while (choice != "0");
        }

        // ========== ФУНКЦИИ ПУНКТОВ МЕНЮ ==========

        static void ShowLaboratories(DatabaseManager db)
        {
            Console.WriteLine("---- Все лаборатории ----");
            var laboratories = db.GetAllLaboratories();
            foreach (var lab in laboratories)
                Console.WriteLine(" " + lab);
            Console.WriteLine($"Итого: {laboratories.Count}");
        }

        static void ShowExperiments(DatabaseManager db)
        {
            Console.WriteLine("---- Все эксперименты ----");
            var experiments = db.GetAllExperiments();
            foreach (var exp in experiments)
                Console.WriteLine(" " + exp);
            Console.WriteLine($"Итого: {experiments.Count}");
        }

        static void AddExperiment(DatabaseManager db)
        {
            Console.WriteLine("---- Добавление эксперимента ----");

            // Показываем лаборатории
            Console.WriteLine("Доступные лаборатории:");
            var laboratories = db.GetAllLaboratories();
            foreach (var lab in laboratories)
                Console.WriteLine(" " + lab);

            // Запрос ID лаборатории
            Console.Write("ID лаборатории: ");
            if (!int.TryParse(Console.ReadLine(), out int labId))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            // Запрос названия
            Console.Write("Название эксперимента: ");
            string name = Console.ReadLine()?.Trim() ?? "";
            if (name.Length == 0)
            {
                Console.WriteLine("Ошибка: название не может быть пустым.");
                return;
            }

            // Запрос длительности
            Console.Write("Длительность (часы): ");
            if (!int.TryParse(Console.ReadLine(), out int duration))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            try
            {
                var exp = new Experiment(0, labId, name, duration);
                db.AddExperiment(exp);
                Console.WriteLine("Эксперимент добавлен.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void EditExperiment(DatabaseManager db)
        {
            Console.WriteLine("---- Редактирование эксперимента ----");
            Console.Write("Введите ID эксперимента: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            var exp = db.GetExperimentById(id);
            if (exp == null)
            {
                Console.WriteLine($"Эксперимент с ID={id} не найден.");
                return;
            }

            Console.WriteLine($"Текущие данные: {exp}");
            Console.WriteLine("(Нажмите Enter, чтобы оставить значение без изменений)");

            // Название
            Console.Write($"Название [{exp.Name}]: ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0)
                exp.Name = input;

            // Лаборатория
            Console.Write($"ID лаборатории [{exp.LaboratoryId}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && int.TryParse(input, out int newLabId))
                exp.LaboratoryId = newLabId;

            // Длительность
            Console.Write($"Длительность [{exp.DurationHours}]: ");
            input = Console.ReadLine()?.Trim() ?? "";
            if (input.Length > 0 && int.TryParse(input, out int newDuration))
            {
                try
                {
                    exp.DurationHours = newDuration;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    return;
                }
            }

            db.UpdateExperiment(exp);
            Console.WriteLine("Данные обновлены.");
        }

        static void DeleteExperiment(DatabaseManager db)
        {
            Console.WriteLine("---- Удаление эксперимента ----");
            Console.Write("Введите ID эксперимента: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Ошибка: введите целое число.");
                return;
            }

            var exp = db.GetExperimentById(id);
            if (exp == null)
            {
                Console.WriteLine($"Эксперимент с ID={id} не найден.");
                return;
            }

            Console.Write($"Удалить «{exp.Name}»? (да/нет): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (confirm == "да")
            {
                db.DeleteExperiment(id);
                Console.WriteLine("Эксперимент удалён.");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
        }

        // ========== ПОДМЕНЮ ОТЧЁТОВ ==========

        static void ReportsMenu(DatabaseManager db)
        {
            string choice;
            do
            {
                Console.WriteLine("--- Отчёты ---");
                Console.WriteLine(" 1 - Список экспериментов по лабораториям");
                Console.WriteLine(" 2 - Количество экспериментов в лабораториях");
                Console.WriteLine(" 3 - Средняя длительность экспериментов по лабораториям");
                Console.WriteLine(" 0 - Назад");
                Console.Write("Ваш выбор: ");

                choice = Console.ReadLine()?.Trim() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": Report1_ExperimentsWithLaboratories(db); break;
                    case "2": Report2_CountByLaboratory(db); break;
                    case "3": Report3_AvgDurationByLaboratory(db); break;
                    case "0": break;
                    default: Console.WriteLine("Неверный пункт."); break;
                }
            } while (choice != "0");
        }

        // Отчёт 1: Эксперименты с названиями лабораторий (JOIN)
        static void Report1_ExperimentsWithLaboratories(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT e.exp_name, l.lab_name, e.exp_duration 
                         FROM exp e 
                         JOIN lab l ON e.lab_id = l.lab_id 
                         ORDER BY e.exp_name")
                .Title("Эксперименты по лабораториям")
                .Header("Эксперимент", "Лаборатория", "Часы")
                .ColumnWidths(30, 30, 10)
                .Numbered()   // нумерация строк (группа А)
                .Footer("экспериментов")  // итоговая строка (группа В)
                .Print();
        }

        // Отчёт 2: Количество экспериментов по лабораториям (GROUP BY + COUNT)
        static void Report2_CountByLaboratory(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT l.lab_name, COUNT(*) AS cnt 
                         FROM exp 
                         JOIN lab l ON exp.lab_id = l.lab_id 
                         GROUP BY l.lab_name 
                         ORDER BY l.lab_name")
                .Title("Количество экспериментов по лабораториям")
                .Header("Лаборатория", "Кол-во экспериментов")
                .ColumnWidths(30, 20)
                .Print();
        }

        // Отчёт 3: Средняя длительность экспериментов по лабораториям (GROUP BY + AVG)
        static void Report3_AvgDurationByLaboratory(DatabaseManager db)
        {
            new ReportBuilder(db)
                .Query(@"SELECT l.lab_name, ROUND(AVG(e.exp_duration), 1) AS avg_duration 
                         FROM exp e 
                         JOIN lab l ON e.lab_id = l.lab_id 
                         GROUP BY l.lab_name 
                         ORDER BY avg_duration DESC")
                .Title("Средняя длительность экспериментов (часы)")
                .Header("Лаборатория", "Среднее часов")
                .ColumnWidths(30, 15)
                .Print();
        }
    }
}