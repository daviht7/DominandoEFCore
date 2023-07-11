using System.Diagnostics;
using System;
using Curso.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace DominandoEFCore
{
  class Program
  {
    static void Main(string[] args)
    {
      //EnsureCreatedAndDeleted();
      //GapDoEnsureCreated();
      //HealthCheckBancoDeDados();
      //_count = 0;
      //GerenciarEstadoConexao(false);
      //_count = 0;
      //GerenciarEstadoConexao(true);
      MigracoesPendentes();
    }

    static void MigracoesPendentes()
    {

      using var db = new ApplicationContext();

      var MigracoesPendentes = db.Database.GetPendingMigrations();

      Console.WriteLine($"Total {MigracoesPendentes.Count()}");

      foreach (var migracao in MigracoesPendentes)
      {

      }

    }

    static void ExecuteSQL()
    {

      using var db = new ApplicationContext();

      using var cmd = db.Database.GetDbConnection().CreateCommand();
      cmd.CommandText = "SELECT 1";
      cmd.ExecuteNonQuery();

      var descricao = "teste";
      db.Database.ExecuteSqlRaw("update departamentos set descricao ={0} where id=1", descricao);

      db.Database.ExecuteSqlInterpolated($"update departamentos set descricao ={descricao} where id=1");

    }

    static void HealthCheckBancoDeDados()
    {
      using var db = new ApplicationContext();
      var canConnect = db.Database.CanConnect();

      if (canConnect)
      {
        Console.WriteLine("Posso me conectar");
      }
      else
      {
        Console.WriteLine("Não posso me conectar");
      }
    }

    static int _count;

    static void GerenciarEstadoConexao(bool gerenciarEstadoConexao)
    {

      using var db = new ApplicationContext();
      var time = Stopwatch.StartNew();

      var conexao = db.Database.GetDbConnection();
      conexao.StateChange += (_, __) => ++_count;

      if (gerenciarEstadoConexao)
      {
        conexao.Open();
      }

      for (int i = 0; i < 200; i++)
      {
        db.Departamentos.AsNoTracking().Any();
      }

      time.Stop();

      var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, {_count}";

      Console.WriteLine(mensagem);

    }

    static void EnsureCreatedAndDeleted()
    {
      using var db = new ApplicationContext();
      //   db.Database.EnsureCreated();
      db.Database.EnsureDeleted();
    }

    static void GapDoEnsureCreated()
    {
      using var db1 = new ApplicationContext();
      using var db2 = new ApplicationContextCidade();
      db1.Database.EnsureCreated();
      db2.Database.EnsureCreated();

      var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
      databaseCreator.CreateTables();
    }

  }
}
