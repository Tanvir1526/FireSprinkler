# Fire Sprinkler

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen.svg)]()

A fire protection engineering solution that calculates optimal sprinkler placement and water pipe connections for building safety compliance.

---

## Table of Contents

- [Overview](#overview)
- [Problem Statement](#problem-statement)
- [System Design](#system-design)
  - [Architecture Overview](#architecture-overview)
  - [Solution Flow](#solution-flow)
  - [Core Algorithms](#core-algorithms)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Sample Output](#sample-output)
- [Design Patterns](#design-patterns)
- [Testing](#testing)

---

## Overview

This application automates fire sprinkler system design by calculating:
- **Sprinkler Quantity**: Number of sprinklers required to cover a room
- **Sprinkler Positions**: Exact 3D coordinates (x, y, z) on the ceiling
- **Pipe Connections**: Optimal connection points to the nearest water supply pipe

---

## Problem Statement
<img width="1000" height="776" alt="image" src="https://github.com/user-attachments/assets/a7f10ffb-5c34-4865-8144-93fc28038106" />

---

## System Design

### Architecture Overview

The solution implements **Clean Architecture** with clear separation of concerns:

```mermaid
flowchart TB
    subgraph Presentation["Presentation Layer"]
        Console["Console App"]
        Formatter["Output Formatter"]
    end

    subgraph Application["Application Layer"]
        UseCase["CalculateSprinklerLayoutUseCase"]
        Strategy["IPlacementStrategy"]
        Factory["PlacementStrategyFactory"]
    end

    subgraph Domain["Domain Layer"]
        Entities["Entities"]
        Services["Domain Services"]
        ValueObjects["Value Objects"]
        Abstractions["Abstractions"]
    end

    subgraph Infrastructure["Infrastructure Layer"]
        GeometryService["GeometryService"]
    end

    Console --> UseCase
    UseCase --> Strategy
    UseCase --> Services
    Strategy --> Services
    Services --> Abstractions
    GeometryService -.->|implements| Abstractions
```

---

### Solution Flow

The system processes sprinkler layout calculations through these steps:

```mermaid
sequenceDiagram
    autonumber
    participant C as Console
    participant UC as UseCase
    participant PS as PlacementStrategy
    participant SP as SprinklerPlacementService
    participant GS as GeometryService
    participant PC as PipeConnectionService

    C->>UC: Execute(room, pipes, wallOffset, spacing)
    UC->>PS: PlaceSprinklers(room, wallOffset, spacing)
    PS->>SP: PlanSprinklers(room, wallOffset, spacing)
    SP->>GS: InsetPolygon(ceilingCorners, wallOffset)
    GS-->>SP: Inset polygon (shrunk boundary)
    SP->>GS: GetBoundingBox(insetPolygon)
    GS-->>SP: Min/Max coordinates
    loop For each grid point
        SP->>GS: IsPointInsidePolygon(point, insetPolygon)
        GS-->>SP: true/false
    end
    SP-->>UC: List of Sprinklers
    UC->>PC: ConnectSprinklersToPipes(sprinklers, pipes)
    loop For each sprinkler
        PC->>PC: Find nearest pipe (min distance)
        PC->>PC: Calculate connection point
    end
    PC-->>UC: Sprinklers with connections
    UC-->>C: SprinklerLayoutResult
```

---

### Core Algorithms

#### 1. Polygon Inset Algorithm

Creates a safe placement boundary by shrinking the room perimeter:

```
┌─────────────────────────────────────┐
│  Original Ceiling Boundary          │
│    ┌─────────────────────────────┐  │
│    │  ← 2500mm offset            │  │
│    │    Inset Boundary           │  │
│    │      (Safe Zone)            │  │
│    │                             │  │
│    └─────────────────────────────┘  │
└─────────────────────────────────────┘
```

**Steps:**
1. Calculate edge vectors between consecutive vertices
2. Compute inward-pointing normals (perpendicular rotation)
3. Calculate bisector at each corner
4. Adjust offset distance based on corner angle
5. Validate resulting polygon has positive area

---

#### 2. Grid-Based Sprinkler Placement

Generates candidate positions using an axis-aligned grid:

```
Bounding Box (Min → Max)
┌───────────────────────────┐
│  ●     ●     ●     ●     │ ← Grid points
│     ●     ●     ●        │   at 2500mm spacing
│  ●     ●     ●     ●     │
│     ●     ●     ●        │
└───────────────────────────┘
● = Candidate position (validated via ray-casting)
```

**Algorithm:**

```
FOR x = min.X TO max.X STEP spacing
  FOR y = min.Y TO max.Y STEP spacing
    IF IsPointInsidePolygon(x, y) THEN
      ADD Sprinkler(x, y, ceilingZ)
```

---

#### 3. Point-in-Polygon Detection (Ray Casting)

Determines if a sprinkler position falls within the valid ceiling area:

```
     Ray →
─────────────────────────●────────────→
          ╱╲          ╱   ╲
         ╱  ╲   ●    ╱     ╲
        ╱    ╲      ╱       ╲
       ╱      ╲────╱         ╲

Odd intersections  = INSIDE  ✓
Even intersections = OUTSIDE ✗
```

---

#### 4. Nearest Pipe Connection

For each sprinkler, finds the optimal connection point using line-segment projection:

```
Pipe Segment:  A ────────────────── B
                        ↑
                        │ Perpendicular
                        │ projection
                        ●
                    Sprinkler
```

**Formula:**

```
t                = clamp((P - A) · (B - A) / |B - A|², 0, 1)
ConnectionPoint  = A + t × (B - A)
```

---

## Project Structure

```
FireSprinklerDesign/
├── src/
│   ├── FireSprinklerDesign.Domain/          # Core business logic
│   │   ├── Abstractions/                    # Interface contracts
│   │   │   ├── IGeometryService.cs
│   │   │   ├── IPipeConnectionService.cs
│   │   │   └── ISprinklerPlanner.cs
│   │   ├── Entities/                        # Domain entities
│   │   │   ├── Pipe.cs
│   │   │   ├── Room.cs
│   │   │   └── Sprinkler.cs
│   │   ├── Services/                        # Domain services
│   │   │   ├── PipeConnectionService.cs
│   │   │   └── SprinklerPlacementService.cs
│   │   └── ValueObjects/                    # Immutable types
│   │       ├── Point3D.cs
│   │       ├── Vector3D.cs
│   │       └── LineSegment3D.cs
│   │
│   ├── FireSprinklerDesign.Application/     # Use cases & orchestration
│   │   ├── DTOs/
│   │   ├── Strategies/                      # Placement algorithms
│   │   │   ├── IPlacementStrategy.cs
│   │   │   ├── GridPlacementStrategy.cs
│   │   │   └── PlacementStrategyFactory.cs
│   │   └── UseCases/
│   │       └── CalculateSprinklerLayoutUseCase.cs
│   │
│   ├── FireSprinklerDesign.Infrastructure/  # External implementations
│   │   └── Services/
│   │       └── GeometryService.cs           # Computational geometry
│   │
│   └── FireSprinklerDesign.Console/         # Entry point
│       ├── DependencyInjection/
│       ├── Output/
│       │   └── ConsoleOutputFormatter.cs
│       └── Program.cs
│
└── tests/
    └── FireSprinklerDesign.Tests/           # Unit & integration tests
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Build & Run

```bash
# Clone repository
git clone https://github.com/Tanvir1526/FireSprinkler.git
cd FireSprinkler

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run application
dotnet run --project src/FireSprinklerDesign.Console
```

### Run Tests

```bash
dotnet test
```

---

## Sample Output


<img width="641" height="979" alt="image" src="https://github.com/user-attachments/assets/13a529c9-27b9-4567-954f-ead70ac7ad83" />
<img width="640" height="971" alt="image" src="https://github.com/user-attachments/assets/b27d737c-11cc-4d51-bc8c-2596cc333d0f" />

---

## Design Patterns

| Pattern | Implementation | Purpose |
|---------|----------------|---------|
| **Clean Architecture** | 4-layer project structure | Separation of concerns, testability |
| **Strategy Pattern** | `IPlacementStrategy` | Interchangeable placement algorithms |
| **Factory Pattern** | `PlacementStrategyFactory` | Strategy instantiation & selection |
| **Dependency Injection** | `ServiceCollection` | Loose coupling, configurability |
| **Value Objects** | `Point3D`, `Vector3D`, `LineSegment3D` | Immutability, domain modeling |
| **Repository Pattern** | Domain abstractions | Infrastructure independence |

---

## Testing

The solution includes comprehensive unit tests covering:

- **Domain Logic**: Entity behavior, value object operations
- **Geometry Calculations**: Point-in-polygon, polygon inset, distance calculations
- **Service Layer**: Sprinkler placement, pipe connection algorithms
- **Edge Cases**: Empty rooms, sharp corners, boundary conditions

---
---

<p align="center">
  <sub>Built with precision engineering for fire safety compliance.</sub>
</p>
