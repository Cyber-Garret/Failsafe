using System.Collections.Generic;
using System.Threading.Tasks;
using BungieAPI.Definitions;
using BungieAPI.Definitions.Seasons;
using BungieAPI.Definitions.Sockets;

namespace BungieAPI
{
  public interface IManifest
  {
    Task<DestinyClassDefinition> LoadClass(uint hash);
    Task<DestinyInventoryItemDefinition> LoadInventoryItem(uint hash);
    Task<IEnumerable<DestinyInventoryItemDefinition>> LoadInventoryItemsWithCategory(uint categoryHash);
    Task<IEnumerable<DestinyInventoryItemDefinition>> LoadInventoryItems(IEnumerable<uint> hashes);
    Task<DestinyInventoryBucketDefinition> LoadBucket(uint hash);
    Task<IEnumerable<DestinyItemCategoryDefinition>> LoadItemCategories(IEnumerable<uint> hashes);
    Task<DestinyInventoryItemDefinition> LoadPlug(uint hash);
    Task<DestinySocketTypeDefinition> LoadSocketType(uint hash);
    Task<DestinySocketCategoryDefinition> LoadSocketCategory(uint hash);
    Task<DestinyStatDefinition> LoadStat(uint hash);
    Task<IEnumerable<DestinyStatDefinition>> LoadStats(IEnumerable<uint> hashes);
    Task<string> GetJson(string tableName, uint hash);
    Task<IEnumerable<string>> GetJson(string tableName, IEnumerable<uint> hashes);
    Task<DestinySeasonDefinition> LoadSeason(uint hash);
    Task<DestinySeasonPassDefinition> LoadSeasonPass(uint hash);
    Task<DestinyProgressionDefinition> LoadProgression(uint hash);
    Task<DestinyVendorDefinition> LoadVendor(uint hash);
    Task<DestinySandboxPerkDefinition> LoadSandboxPerk(uint hash);
    Task<IEnumerable<DestinySandboxPerkDefinition>> LoadSandboxPerks(IEnumerable<uint> hashes);
    Task<DestinyDamageTypeDefinition> LoadDamageType(uint hash);
    Task<DestinyStatDefinition> LoadStatType(uint hash);
    Task<IEnumerable<DestinyStatDefinition>> LoadStatTypes(IEnumerable<uint> hashes);
  }
}