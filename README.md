Note: this archive goes against Dreadrith's, the Original Author, wishes. It is however compliant with the original OS licenses.

<div align="center">

# DreadScripts-Archive

</div>

This repository is for archiving some of Dreadrith's Open-Source scripts that we thought didn't need their own repo, but should still have a central place to be kept for software preservation purposes.

### Asset Organizer

A Unity editor tool that automatically reorganizes Prefabs, Scenes, and their dependencies into categorized folders. It supports custom folder rules for different asset types and can clean up empty folders. Ideal for decluttering projects and maintaining consistent asset structures. Includes options to define sorting behaviors for textures, scripts, and other common asset types.
### AutoBounds

Optimizes skinned mesh renderer bounds in VR avatars to prevent models from disappearing at certain angles. Offers both automatic calculation and manual sampling from reference objects. Particularly useful for non-T-pose avatars where default bounds may cause visibility issues. Helps ensure full-body avatars remain visible in peripheral vision.
### Better-Unity

A collection of Unity editor enhancements, including asset copy/paste, quick text file creation, and transform utilities. Features individual field copying for transforms and GameObject path copying for easier hierarchy navigation. Designed to streamline repetitive tasks and improve workflow efficiency. Also includes GUID copying for advanced asset management.
### Camera Utility

Quickly synchronizes Unity’s Scene view and Game view cameras with a single click. Useful for previewing in-game camera angles while editing or debugging. Simplifies alignment for cinematics, UI placement, and VR development. Works seamlessly without additional setup.
### CopyCutPaste

Adds standard copy, cut, and paste functionality to Unity’s asset context menu. Eliminates the need for drag-and-drop or manual duplication of assets. Speeds up file organization and project structuring. A simple but essential quality-of-life improvement.
### Limb Control

Enables desktop VR users to animate limbs via VRChat’s Puppet Control system. Supports custom BlendTrees for personalized movement styles. Includes tracking toggle options for individual body parts. Great for half-body or full-body avatars needing manual limb animation.

### Metal-Pipe

Adds an editor window that plays a metal pipe sound either when clicked, or every so often depending on the settings.

### NoAutoGame

Prevents Unity’s Game window from auto-opening or stealing focus during playmode. Configurable to either close the Game window entirely or just refocus the original Scene view. Reduces disruptions when testing or debugging. Especially helpful for multi-monitor setups.
### Package Processor

Simplifies Unity package imports/exports with smart filtering rules. Automatically excludes unwanted file types (e.g., scripts, meta files) during exports. Supports per-folder, extension, or asset-type exclusions. Saves time when sharing or migrating project assets.
### PhysBone Converter

Converts VRChat PhysBones to DynamicBones (or vice versa) with near-equivalent settings. Handles colliders and basic physics properties during conversion. Useful for avatars needing backward compatibility or testing different physics systems. Not perfect but covers most common use cases.
### Quick Toggle

Generates animation toggle clips for GameObjects, components, or blendshapes in seconds. Supports batch creation, opposite-state clips, and customizable durations. Ideal for fast prototyping of avatar toggles or facial expressions. Reduces manual animation workflow overhead.
### Replace Motion

Bulk-replaces animation clips in avatars or Animator Controllers. Helps swap motions (e.g., idle animations) across multiple objects at once. Displays a list of all used motions for easy selection. Saves time when updating or standardizing animations.
### Reset Humanoid

Resets an avatar’s pose to T-stance with optional position/rotation/scale adjustments. Useful for fixing misaligned rigs or preparing models for animation. Works on humanoid characters with minimal setup.
### SelectionHelper

Enhances object selection in Unity with filters for types, hierarchies, and saved selections. Includes a scene overlay for quickly picking bones or hidden objects. Great for complex scenes with deep hierarchies.
### Text2Texture

Generates textures (normal maps, masks) or materials from text input. Supports custom fonts and styling for branding or UI elements. Exports textures for use in shaders or decals. Requires TextMeshPro for font rendering.
### Texture Utility

Edits textures directly in Unity—resize, repack channels, recolor, or generate gradients. Includes masking and auto-packing for optimized texture workflows. Eliminates the need for external image editors for basic adjustments.

### VRCProcessor

Automates VR avatar import settings: enforces humanoid rig, enables read/write, and removes problematic bones (e.g., jaw). Ensures models meet VRChat’s requirements on import.
