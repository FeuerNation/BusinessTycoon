﻿#region Info
// -----------------------------------------------------------------------
// MarketManager.cs
// 
// Felix Jung 07.06.2023
// -----------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using BT.Scripts.production;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace BT.Scripts.Gameplay {
  public class MarketManager : SerializedMonoBehaviour {

    public ProductSo[] products;

    [OdinSerialize]
    private List<Offer> offers;

    [OdinSerialize]
    private Dictionary<ProductSo, int> productDemand
        = new Dictionary<ProductSo, int>();

    public void Initialize() {
      offers = new List<Offer>();

      //TODO: remove this test code
      SetProductDemand(products[0], 500);
      SetProductDemand(products[1], 10);

      Debug.Log("Demand for " + products[0].name + " is " +
                GetProductDemand(products[0]));
      Debug.Log("Demand for " + products[1].name + " is " +
                GetProductDemand(products[1]));

      if (Debug.isDebugBuild) { Debug.Log("MarketManager initialized"); }

    }

    public void SetProductDemand(ProductSo productType, int demand) {
      productDemand[productType] = demand;

    }

    public int GetProductDemand(ProductSo productType) {
      if (productDemand.TryGetValue(productType, out int demand)) {
        return demand;
      } else {
        throw new Exception("Product" + productType +
                            "not found in productDemand");
      }
    }

    public void UpdateMarket() {
      //TODO: improve this logic to be more efficient
      var products = new List<ProductSo>(productDemand.Keys);

      foreach (var productType in products) {
        int remainingDemand = productDemand[productType];
        List<Offer> offersForProduct
            = offers.FindAll(offer => offer.product.type == productType &&
                                      !offer.isSold);
        SortOffersByScore(offersForProduct);


        foreach (var offer in offersForProduct) {
          int quantityToBuy = Mathf.Min(remainingDemand, offer.quantity);
          remainingDemand -= quantityToBuy;


          offer.soldQuantity = quantityToBuy;
          offer.isSold = true;
          // Remove products from company
          offer.company.RemoveProduct(offer.product.type, quantityToBuy);
          offer.soldQuantity = quantityToBuy;
          offer.quantity -= quantityToBuy;
          // Add money to company
          offer.company.AddMoney(quantityToBuy * offer.price);
          //remove offer if it is empty
          if (offer.quantity <= 0) {
            offers.Remove(offer);
            Debug.Log("Removed offer from market");
          }
          
          Debug.Log("Sold " + quantityToBuy + " of " + offer.product.type.name +
                    " for " + offer.price + " each");

          if (remainingDemand <= 0) { break; }
        }

        productDemand[productType] = remainingDemand;
      }

      AdjustPrices();
      Debug.Log("UpdateMarket");
    }

    private void SortOffersByScore(List<Offer> offers) {
      offers.Sort((a, b) => a.price.CompareTo(b.price));
      //TODO: Add more factors and sorting logic in the future
    }

    private void AdjustPrices() {
      // TODO: Implement price adjustment logic based on supply and demand
    }

    public void ClearOffers() {
      offers.Clear();
    }

    public void AddOffer(Offer offer) {
      offers.Add(offer);
    }

    public decimal GetSales(Company company) {
      decimal sales = 0;

      foreach (var offer in offers) {
        if (offer.company == company && offer.isSold) {
          sales += offer.price * offer.soldQuantity;
        }
      }

      return sales;
    }

    public IReadOnlyList<Offer> GetOffers() {
      return offers.AsReadOnly();
    }
  }
}