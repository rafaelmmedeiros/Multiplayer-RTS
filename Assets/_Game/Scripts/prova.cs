using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class prova : MonoBehaviour
{
}

public class Machine
{
    public List<Slot> Slots { get; }

    public int TotalDeviation => Slots.Sum(s => s.Deviation);

    public void Suppy(List<ProductSupply> productsToSupply)
    {
        foreach (ProductSupply productToSupply in productsToSupply) // Do each supply
        {
            // List of slot from this product ordered by deviation
            List<Slot> productSlotsByDeviation = Slots.FindAll(x => x.ProductId == productToSupply.ProductId)
                                                      .OrderByDescending(o => o.Deviation)
                                                      .ToList();

            if (productSlotsByDeviation == null)
                throw new Exception($"ERRO: Id de produto não existe na maquina! id: {productToSupply.ProductId}");

            int productQuantityToSupply = productToSupply.Quantity;

            foreach (Slot slot in productSlotsByDeviation) // Try each slot
            {
                FillSlot(slot, slot.DesiredQuantity, ref productQuantityToSupply); // FILL SLOT TO DESIRED
            }

            if (productQuantityToSupply <= 0) continue;

            foreach (Slot slot in productSlotsByDeviation) // Try each slot again
            {
                FillSlot(slot, slot.Capacity, ref productQuantityToSupply); // FILL SLOT TO CAPACITY
            }

            if (productQuantityToSupply > 0)
                throw new Exception($"ERRO: Sobrou produtos! id: {productToSupply.ProductId}, Quantidade: {productQuantityToSupply}");

        }
    }

    private void FillSlot(Slot slot, int threshold, ref int productQuantityToSupply)
    {
        // It will fill the Slot if it has space to fill
        if (slot.CurrentQuantity < threshold)
        {
            // Guarantees never to go beyond the threshold.
            int quantityToSupply = Math.Min(threshold - slot.CurrentQuantity, productQuantityToSupply);

            // Update the productQuantityToSupply.
            productQuantityToSupply -= quantityToSupply;

            // Update Slot Quantity
            slot.CurrentQuantity += quantityToSupply;
        }
    }
}

public class Slot
{
    public int ProductId { get; }
    public int CurrentQuantity { get; set; }
    public int DesiredQuantity { get; }
    public int Capacity { get; }

    public int Deviation => Math.Abs(DesiredQuantity - CurrentQuantity);
}

public class ProductSupply
{
    public int ProductId { get; }
    public int Quantity { get; }
}
