# Toolkit Overview

## Core
**toolkit.core** is a modular toolkit designed to streamline and enhance game development workflows. It offers a suite of utilities and systems aimed at reducing boilerplate code, promoting clean architecture, and facilitating rapid prototyping.

### Key Features
**UnityCondition & UnityEvaluator**
These components enable the creation of data-driven conditional logic within the Unity Editor. *UnityCondition* allows developers to define conditions based on game data or hard-coded values, triggering OnTrue or OnFalse events accordingly. *UnityEvaluator* complements this by evaluating expressions to facilitate dynamic decision-making processes. 

**Object Spawning**
Provides mechanisms for spawning objects within the game world, supporting features like controlled instantiation, management of spawned instances, and integration with other systems for interactive object placement. 

**TimedCurve System**
Implements a component for managing tweening using curves. This allows for smooth interpolation of values over time, facilitating effects such as easing, pulsing, or other temporal behaviors.

https://github.com/nrvllrgrs/toolkit.core

## AI
https://github.com/nrvllrgrs/toolkit.ai

## Dialogue
**toolkit.dialogue** is designed to integrate Yarn Spinner's narrative scripting capabilities into Unity projects — adding priority and queuing support. It builds upon the foundational features provided by toolkit.core, offering developers a streamlined approach to implementing rich, branching dialogues within their games.

### Key Features
**Dialogue Types & Priorities**
Supports multiple dialogue types (e.g. monologue, conversation, ambient) with assigned priorities, ensuring critical dialogues can interrupt or override less important ones.

**Dialogue Queuing System**
Implements a queuing mechanism that manages multiple dialogue requests, displaying them sequentially based on priority and context.

**Nudges & Ambient Dialogue**
Facilitates non-intrusive dialogue prompts or "nudges" that can provide hints or flavor text without disrupting gameplay flow.

https://github.com/nrvllrgrs/toolkit.dialogue

## Health
**toolkit.health** is a flexible and extensible health management system for Unity, designed to support a wide range of gameplay mechanics from simple damage tracking to complex layered health models.

### Key Features
**Layered Health Architecture**
Define multiple health layers such as shields, armor, and core health. Each layer can have unique behaviors (e.g. shields regenerate, armor reduces incoming damage, and health is affected last).

**Armor & Damage Reduction Systems**
Integrate armor values that mitigate incoming damage based on configurable rules, such as flat reduction or percentage-based scaling. Easily extend this to include resistances or vulnerability modifiers.

**Custom Damage Types**
Support different damage types (e.g. fire, poison, physical) and selectively apply effects based on layer or entity type. Useful for RPGs, tactical games, and ability-based systems.

https://github.com/nrvllrgrs/toolkit.health

## Inventory
**toolkit.inventory** is a modular inventory system. It provides a flexible, data-driven framework for managing items, crafting recipes, currencies, and loot tables.

### Key Features
**Item Management**
Supports the creation and management of various item types, including stackable and non-stackable items, with customizable properties and behaviors.

**Crafting Recipes**
Allows for the definition of crafting recipes, enabling players to combine items to create new ones. Recipes can be configured to require specific items and quantities.

**Currency System**
Implements a currency system to handle in-game economies, facilitating transactions like buying, selling, and trading items.

**Loot Tables**
Provides a mechanism to define loot tables, specifying the probability and quantity of items dropped by enemies or found in containers.


https://github.com/nrvllrgrs/toolkit.inventory

## Material FX
**toolkit.materialfx** is designed to dynamically spawn audiovisual effects (SFX, VFX, decals, etc.) based on material interactions. Rather than relying on rigid, surface-specific code, it provides a data-driven system that reacts contextually to what objects are made of and how they collide.

### Key Features
**Material-Driven FX Mapping**
Define interactions between different physical materials (e.g. metal hitting wood vs. metal hitting stone) and assign specific visual and audio effects for each pair.

**Dynamic FX Instantiation**
Automatically spawns context-appropriate sound effects, particle systems, or decals at runtime without needing to hardcode per-object logic.

https://github.com/nrvllrgrs/toolkit.materialfx

## Quest
**toolkit.quest** provides a structured framework for implementing complex quest mechanics, supporting features like sequential and parallel task execution, and scene-specific quest management.

### Key Features
**Quests vs. Tasks**
Differentiates between overarching quests and individual tasks. Tasks can be configured to execute in sequence, requiring completion in a specific order, or in parallel, allowing multiple tasks to be completed simultaneously.

**Quest Layer Scene Management**
Implements a dedicated "Quest Layer" to manage quest-related objects and logic within specific scenes — allowing for dynamic loading and unloading of quest components based on the player's progression and current scene context.

https://github.com/nrvllrgrs/toolkit.quest

## Scene
In Progress

## Sensors
**toolkit.sensors** is a system for equipping game objects with sensing and perception capabilities. It enables designers and developers to simulate environmental awareness through customizable sensors that detect, filter, and evaluate stimuli in real time.

### Key Features
**Multiple Sensor Modalities**
Includes raycast, trigger zone, and other detection methods, supporting both directional and area-based sensing.

**Extensible Sensor Filters**
Sensors can be customized with filter components that define what types of signals they detect. This allows developers to layer logic—such as team-based detection, object type, or custom tags—making the system adaptable for a wide variety of gameplay needs.

**Signal Strength Scoring**
Detected signals can include a strength parameter, enabling scoring systems that rank detections based on proximity, visibility, or other environmental factors. This is useful for AI decision-making, priority targeting, or stealth mechanics.

https://github.com/nrvllrgrs/toolkit.sensors

## Shooter
**toolkit.shooter** is designed for implementing a wide range of shooting mechanics — from precision raycast weapons to dynamic projectile systems.

### Key Features
**Raycast & Projectile Weapon**
Easily configure hitscan (raycast) weapons like rifles or lasers, or physical projectiles like grenades, rockets, and arrows — each with their own behaviors and collision logic.

**Impact vs Splash Damage Handling**
Define direct-impact damage for single-target hits or area-of-effect (AoE) splash damage with configurable radius and falloff.

**Customizable Firing Modes**
Support for single-shot, burst-fire, and automatic weapons. Timing, spread, and reload logic are all extensible and component-based.

**Modular Weapon Components**
Weapons are composed of pluggable components (e.g. cooldown, ammo), making it easy to create new weapon types without rewriting core systems.

https://github.com/nrvllrgrs/toolkit.shooter

## Status FX
**toolkit.statusfx** is a system designed to manage and apply status effects—such as buffs, debuffs, and condition-based modifiers—to game entities.

### Key Features
**Modular Status Effects**
Define and manage various status effects (e.g., poison, stun, haste) that can be applied to game entities. Each effect can have unique behaviors, durations, and stacking rules, allowing for intricate gameplay dynamics.

**Effect Lifecycle Management**
Handles the complete lifecycle of status effects, including application, duration tracking, periodic updates, and removal. This ensures consistent behavior and simplifies the management of complex effect interactions.

https://github.com/nrvllrgrs/toolkit.statusfx

## Tabletop
https://github.com/nrvllrgrs/toolkit.tabletop

## UI
In Progress

## Vehicle
https://github.com/nrvllrgrs/toolkit.vehicles

## Vision
**toolkit.vision** is a flexible, data-driven system for implementing visual modes in Unity — such as thermal vision, night vision, x-ray, and other perception overlays.

### Key Features
**Data-Driven Vision Modes**
Vision modes are configured via ScriptableObjects, enabling non-programmers to define unique visual effects, shaders, and behaviors without modifying code.

https://github.com/nrvllrgrs/toolkit.vision

## XP
**toolkit.xp** is a modular experience and leveling system for Unity, designed to integrate. It provides a flexible, data-driven framework for managing player progression.

### Key Features
**Level Curve Configuration**
Supports customizable level curves, allowing developers to define XP requirements for each level. This enables the creation of tailored progression systems that can scale in complexity as needed.

https://github.com/nrvllrgrs/toolkit.xp

## XR Interaction+
**toolkit.xri** is a modular extension for Unity's XR Interaction Toolkit (XRI), designed to streamline the development of immersive VR and AR experiences. It provides a flexible, data-driven framework, facilitating the creation of complex interaction systems.

### Key Features
**Holsters & Pockets System**
Create immersive physical storage systems where players can stow and retrieve items from configurable holsters and body pockets — ideal for inventory, weapons, and tools.

**Input Action Events**
A flexible event system built on Unity's Input System that allows developers to respond to user inputs with context-aware, runtime-bound logic. Great for gestures, tool use, or custom controls.

**Extended Interactors & Interactables**
Build on the XR Interaction Toolkit with custom behaviors, constraints, and targeting logic to support more advanced and context-sensitive interactions.

https://github.com/nrvllrgrs/toolkit.xri