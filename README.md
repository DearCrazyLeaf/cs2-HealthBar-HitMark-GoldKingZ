> **HLYM Edition** – 非官方维护版本，保留原版 README 说明，仅补充：
> - 使用 GameHUD API 重写 HitMark（图标/伤害独立通道，新增偏移 + 字体/颜色配置）。
> - 移除 HealthBar 功能与旧粒子依赖，保留声音预缓存机制。
> - 扩展配置模板、迁移说明、.gitignore 等开发者体验改进。
> - 所有新增开关与字段详见 `config/config.json` 顶部注释。

## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] HealthBar-HitMark-GoldKingZ (1.0.1)

Show HealthBar , Custom HitMarks , Custom Sounds

![healthbar](https://github.com/user-attachments/assets/25e88501-00c8-4fea-b7fd-7324d79048db)
![hitmark](https://github.com/user-attachments/assets/0a7a6e2b-79f8-4102-92ef-67473db1138d)

---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2.x-2d2d2d?logo=sourceengine)](https://www.sourcemm.net/downloads.php?branch=dev)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

---

## 📥 Installation

1. Download latest release
2. Extract to `csgo` directory
3. Configure `HealthBar-HitMark-GoldKingZ\config\config.json`
4. Restart server

---

## ⚙️ Configuration

> [!NOTE]
> Located In ..\HealthBar-HitMark-GoldKingZ\config\config.json                                           
>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|  
| `HM_EnableHitMark` | Enable HitMark | `true`/`false` | - |  
| `HM_DisableOnWarmUp` | Disable HitMark On WarmUp | `true`/`false` | - |  
| `HM_MuteDefaultHeadShotBodyShot` | Mute default hit sounds when custom sounds exist | `true`/`false` | - |  
| `HM_HeadShotIcon` | Text/icon displayed for headshots | Any short text | `HM_EnableHitMark=true` |  
| `HM_HeadShotColor` | Headshot icon color (`#RRGGBB` or color name) | String | `HM_EnableHitMark=true` |  
| `HM_HeadShotDuration` | Headshot icon duration (seconds) | `float` | `HM_EnableHitMark=true` |  
| `HM_BodyShotIcon` | Text/icon displayed for body shots | Any short text | `HM_EnableHitMark=true` |  
| `HM_BodyShotColor` | Body shot icon color (`#RRGGBB` or color name) | String | `HM_EnableHitMark=true` |  
| `HM_BodyShotDuration` | Body shot icon duration (seconds) | `float` | `HM_EnableHitMark=true` |  
| `HM_FontName` | HUD font family | Installed font name | `HM_EnableHitMark=true` |  
| `HM_FontUnits` | GameHUD world units-per-pixel (smaller = crisper) | `0.05-1.0` | `HM_EnableHitMark=true` |  
| `HM_HitChar` | Glyph rendered for hit confirmation | Single character / short text | `HM_EnableHitMark=true` |  
| `HM_HitScaleStart` | Starting font size for `HM_HitChar` animation | `1-72` | `HM_EnableHitMark=true` |  
| `HM_HitScaleEnd` | Target font size reached by animation | `>= HM_HitScaleStart` | `HM_EnableHitMark=true` |  
| `HM_HitScaleDuration` | How long the scale animation lasts (seconds) | `0.01-1.0` | `HM_EnableHitMark=true` |  
| `HM_HitScaleSteps` | Number of interpolation steps for scaling | `1-10` | `HM_EnableHitMark=true` |  
| `HM_ShowDamageValue` | Append damage digits after the icon | `true`/`false` | `HM_EnableHitMark=true` |  
| `HM_DamageColor` | Damage number color (`#RRGGBB` or color name) | String | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_DamageHeadShotColor` | Damage color override for headshots | String | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_DamageFontSize` | Damage number font size | `8-72` | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_DamageDuration` | Damage number duration (seconds) | `float` | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_DamageOffsetX` | Damage number X offset relative to icon (-30 to 30) | Float | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_DamageOffsetY` | Damage number Y offset relative to icon (-15 to 15) | Float | `HM_EnableHitMark=true` & `HM_ShowDamageValue=true` |  
| `HM_HudOffsetX` | Horizontal HUD offset (-50 to 50 recommended) | Float | `HM_EnableHitMark=true` |  
| `HM_HudOffsetY` | Vertical HUD offset (-10 to 10 recommended) | Float | `HM_EnableHitMark=true` |  
| `HM_HudDistance` | Distance from player view to HUD (40 to 70 recommended) | Float | `HM_EnableHitMark=true` |  
| `HM_HeadShotSounds` | Headshot sound mapping list (`Flag|SoundPath`) | Array of strings | `HM_EnableHitMark=true` |  
| `HM_BodyShotSounds` | Body shot sound mapping list (`Flag|SoundPath`) | Array of strings | `HM_EnableHitMark=true` |  
| `EnableDebug` | Debug mode | `true`/`false` | - |  


---

---

## 📜 Changelog

<details>
<summary>📋 View Version History (Click to expand 🔽)</summary>

### [1.0.1]
- Added CS2_ShowHealthBarTo 3 = Victim Team 4 = Attacker Team
- Added HM_EnableHitMark
- Added HM_DisableOnWarmUp
- Added HM_MuteDefaultHeadShotBodyShot
- Added HM_HeadShot
- Added HM_BodyShot
- Added EnableDebug
- Added In config.json info on each what it do

### [1.0.0]
- Initial Release

</details>

---
