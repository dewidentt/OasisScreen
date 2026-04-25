# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.3] - 2026-04-26

### Changed
- **Фоновый процесс** — Приложение теперь отображается в диспетчере задач как фоновый процесс, а не в разделе «Приложения». Невидимое служебное окно скрыто из Alt+Tab и панели задач через флаги `WS_EX_TOOLWINDOW` / `WS_EX_NOACTIVATE`.

## [1.0.2] - 2026-04-26

### Fixed
- **Multi-monitor overlay** — The screenshot selection form now spans all connected monitors instead of only the primary monitor, allowing region selection across any display

## [1.0.1] - 2026-04-23

### Added
- **Multi-monitor support** — Screenshots now capture all connected monitors in a single image, not just the primary monitor

### Fixed
- Encoding fix for balloon tip notifications

## [1.0.0] - 2024-04-15

### Added
- Screenshot with region selection
- Instant full-screen screenshot with hotkey
- Drawing tools: pencil, arrow, text, shapes, mosaic
- Magnifier tool (like iPhone screenshots)
- Color picker
- Undo/redo support (up to 30 steps)
- Configurable save folder
- Clipboard copy option
- Auto-start with Windows
- System tray integration
