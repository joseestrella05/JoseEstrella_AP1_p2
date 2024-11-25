﻿using JoseEstrella_AP1_p2.DAL;
using JoseEstrella_AP1_p2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JoseEstrella_AP1_p2.Services;

public class ComboService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int comboId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Combos.AllAsync(c => c.ComboId == comboId);

    }

    public async Task<bool> Guardar(Combo Combo)
    {
        if (!await Existe(Combo.ComboId))
        {
            return await Insertar(Combo);
        }
        else
        {
            return await Modificar(Combo);
        }
    }

    public async Task<bool> Insertar(Combo combo)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Combos.Add(combo);
        return await contexto.SaveChangesAsync() > 0;


    }

    public async Task<bool> Modificar(Combo combo)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(combo);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var combos = contexto.Combos.Find(id);
        contexto.ComboDetalles.RemoveRange();
        contexto.Combos.Remove(combos);
        var cantidad = await contexto.SaveChangesAsync();
        return cantidad > 0;
    }

    public async Task<Combo?> Buscar(int id)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Combos.Include(c => c.ComboId == id)
            .FirstOrDefaultAsync(c => c.ComboId == id);
    }

    public async Task<List<Combo>> Listar(Expression<Func<Combo, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Combos.Include(a => a.ComboId)
           .Where(criterio)
           .AsNoTracking()
           .ToListAsync();


    }
   
}
