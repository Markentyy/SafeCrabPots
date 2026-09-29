# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

## [1.0.2] - 2026-09-29

### Added

- Nexus update key so SMAPI notifies players about new versions.

### Fixed

- Docs: the retrieve modifier accepts gamepad buttons too (e.g. `LeftTrigger`); no code changes were needed.

## [1.0.1] - 2026-09-29

### Fixed

- Manifest description referenced the removed tool-dismantling system; rewritten to match the modifier-click design (the text GMCM shows).

## [1.0.0] - 2026-09-29

### Added

- Right-click pickup guard with `Off` / `Modifier` / `Always` modes.
- One-click harvest + auto-rebait from hand (respects Luremaster).
- Configurable retrieve modifier keybind with on-screen hint.
- GMCM options page (soft dependency).
