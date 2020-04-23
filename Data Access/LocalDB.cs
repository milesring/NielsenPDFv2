using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NielsenPDFv2.Models;
using SQLite;

namespace NielsenPDFv2.Data_Access
{
    public class LocalDB
    {
        readonly SQLiteAsyncConnection _db;
        //string dbPath = @"C:\Users\Nielsen\Desktop\db.db";
        string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)+@"\NielsenPDFDB.db";
        public LocalDB()
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Contract>().Wait();
        }

        public Task<int> SaveContractAsync(Contract contract)
        {
            if (contract.ID != 0)
            {
                return _db.UpdateAsync(contract);
            }
            else
            {
                return _db.InsertAsync(contract);
            }
        }

        public Task<int> DeleteContractAsync(Contract contract)
        {
            return _db.DeleteAsync(contract);
        }

        public Task<Contract> GetContractAsync(int id)
        {
            return _db.Table<Contract>()
                .Where(i => i.ID == id)
                .FirstOrDefaultAsync();
        }

        public Task<List<Contract>> GetContractsAsync()
        {
            return _db.Table<Contract>().ToListAsync();
        }


    }
}
