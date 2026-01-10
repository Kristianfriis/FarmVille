using Blazored.LocalStorage;
using FarmVille.Client.Models;

namespace FarmVille.Client.Services;

public class FarmService
{
    private readonly ILocalStorageService _storage;
    private const string ProfileKey = "farm_profile";
    private const string LastSaveKey = "farm_last_save";

    public FarmService(ILocalStorageService storage) => _storage = storage;

    // Generates a key like "farm_slot_1"
    private string GetKey(int slotId) => $"farm_slot_{slotId}";

    public async Task<FarmSaveData?> LoadFarm(int slotId)
    {
        return await _storage.GetItemAsync<FarmSaveData>(GetKey(slotId));
    }

    public async Task SaveFarm(int slotId, FarmSaveData data)
    {
        await _storage.SetItemAsync(GetKey(slotId), data);

        await LastSaveFarm(slotId);
    }

    public async Task DeleteFarm(int slotId)
    {
        await _storage.RemoveItemAsync(GetKey(slotId));
    }

    public async Task LastSaveFarm(int slotId)
    {
        await _storage.SetItemAsync(LastSaveKey, slotId);
    }

    public async Task<int?> LoadLastSaveFarm()
    {
        return await _storage.GetItemAsync<int?>(LastSaveKey);
    }
}