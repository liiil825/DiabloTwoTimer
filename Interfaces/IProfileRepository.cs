using System.Collections.Generic;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface IProfileRepository
{
    // 获取单个角色
    CharacterProfile? GetByName(string name);

    // 获取所有角色名称列表 (用于下拉框)
    List<string> GetAllNames();

    // 获取所有完整角色数据
    List<CharacterProfile> GetAll();

    // 保存 (新建或更新)
    void Save(CharacterProfile profile);

    // 删除
    void Delete(CharacterProfile profile);
}
