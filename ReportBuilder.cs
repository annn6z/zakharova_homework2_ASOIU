using System;
using System.Text;
using System.IO;

namespace LaboratoryExperimentsApp
{
    
    class ReportBuilder
    {
        private DatabaseManager _db;

        // Настройки отчёта
        private string _sql = "";
        private string _title = "";
        private string[] _headers = Array.Empty<string>();
        private int[] _widths = Array.Empty<int>();
        private bool _numbered = false;  // для группы А
        private string _footer = "";      // для группы В

        
        public ReportBuilder(DatabaseManager db)
        {
            _db = db;
        }

        // ========== ПРОМЕЖУТОЧНЫЕ МЕТОДЫ (возвращают this) ==========

        public ReportBuilder Query(string sql) { _sql = sql; return this; }
        public ReportBuilder Title(string title) { _title = title; return this; }
        public ReportBuilder Header(params string[] columns) { _headers = columns; return this; }
        public ReportBuilder ColumnWidths(params int[] widths) { _widths = widths; return this; }
        public ReportBuilder Numbered() { _numbered = true; return this; }
        public ReportBuilder Footer(string label) { _footer = label; return this; }

        // ========== ТЕРМИНАЛЬНЫЕ МЕТОДЫ ==========

       
        public string Build()
        {
            var (columns, rows) = _db.ExecuteQuery(_sql);
            var sb = new StringBuilder();

            // Заголовок
            if (_title.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"=== {_title} ===");
            }

            // Определяем названия колонок
            string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
            int colCount = displayHeaders.Length;

            // Определяем ширину колонок
            int[] widths;
            if (_widths.Length >= colCount)
                widths = _widths;
            else
            {
                widths = new int[colCount];
                for (int i = 0; i < colCount; i++)
                    widths[i] = 20;
            }

            int numWidth = _numbered ? 5 : 0;

            // Шапка таблицы
            if (_numbered)
                sb.Append("№".PadRight(numWidth));
            for (int i = 0; i < colCount; i++)
                sb.Append(displayHeaders[i].PadRight(widths[i]));
            sb.AppendLine();

            // Разделитель
            int totalWidth = numWidth;
            for (int i = 0; i < colCount; i++)
                totalWidth += widths[i];
            sb.AppendLine(new string('-', totalWidth));

            // Строки данных
            for (int r = 0; r < rows.Count; r++)
            {
                if (_numbered)
                    sb.Append((r + 1).ToString().PadRight(numWidth));
                for (int c = 0; c < rows[r].Length && c < colCount; c++)
                    sb.Append(rows[r][c].PadRight(widths[c]));
                sb.AppendLine();
            }

            // Итоговая строка
            if (_footer.Length > 0)
            {
                sb.AppendLine(new string('-', totalWidth));
                sb.AppendLine($"Итого {_footer}: {rows.Count}");
            }

            return sb.ToString();
        }

        
        public void Print()
        {
            Console.WriteLine(Build());
        }

        
        public void SaveToFile(string path)
        {
            File.WriteAllText(path, Build());
            Console.WriteLine($"Отчёт сохранён в файл: {path}");
        }
    }
}