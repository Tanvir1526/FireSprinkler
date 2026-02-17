import plotly.graph_objects as go

# ===============================
# ROOM CORNERS
# ===============================

room_x = [97500.01, 85647.67, 91776.75, 103629.07, 97500.01]
room_y = [34000.00, 43193.61, 51095.16, 41901.55, 34000.00]
room_z = [2500.00, 2500.00, 2530.00, 2530.00, 2500.00]

# ===============================
# WATER PIPES
# ===============================

pipes = [
    [(98242.11, 36588.29, 3000.00), (87970.10, 44556.09, 3000.00)],  # Pipe 1
    [(99774.38, 38563.68, 3000.00), (89502.37, 46531.47, 3000.00)],  # Pipe 2
    [(101306.65, 40539.07, 3000.00), (91034.63, 48507.01, 3000.00)], # Pipe 3
]

# ===============================
# SPRINKLERS
# ===============================

sprinklers = [
    # Pipe 1
    (89155.33, 43636.73, 2507.50),   # S1
    (91130.72, 42104.46, 2507.50),   # S4
    (93106.10, 40572.20, 2507.50),   # S7
    (95081.49, 39039.93, 2507.50),   # S10
    (97056.88, 37507.66, 2507.50),   # S13
    # Pipe 2
    (90687.60, 45612.12, 2515.00),   # S2
    (92662.98, 44079.85, 2515.00),   # S5
    (94638.37, 42547.58, 2515.00),   # S8
    (96613.76, 41015.32, 2515.00),   # S11
    (98589.15, 39483.05, 2515.00),   # S14
    # Pipe 3
    (92219.87, 47587.50, 2522.50),   # S3
    (94195.25, 46055.24, 2522.50),   # S6
    (96170.64, 44522.97, 2522.50),   # S9
    (98146.03, 42990.70, 2522.50),   # S12
    (100121.42, 41458.43, 2522.50),  # S15
]

sprinkler_labels = [
    "S1",  "S4",  "S7",  "S10", "S13",  # Pipe 1
    "S2",  "S5",  "S8",  "S11", "S14",  # Pipe 2
    "S3",  "S6",  "S9",  "S12", "S15",  # Pipe 3
]

# ===============================
# CONNECTIONS (Sprinkler -> Pipe Point)
# ===============================

connections = [
    # Pipe 1 connections
    [(89155.33, 43636.73, 2507.50), (89155.33, 43636.73, 3000.00)],  # S1
    [(91130.72, 42104.46, 2507.50), (91130.71, 42104.46, 3000.00)],  # S4
    [(93106.10, 40572.20, 2507.50), (93106.10, 40572.19, 3000.00)],  # S7
    [(95081.49, 39039.93, 2507.50), (95081.49, 39039.92, 3000.00)],  # S10
    [(97056.88, 37507.66, 2507.50), (97056.87, 37507.65, 3000.00)],  # S13
    # Pipe 2 connections
    [(90687.60, 45612.12, 2515.00), (90687.60, 45612.11, 3000.00)],  # S2
    [(92662.98, 44079.85, 2515.00), (92662.98, 44079.85, 3000.00)],  # S5
    [(94638.37, 42547.58, 2515.00), (94638.37, 42547.58, 3000.00)],  # S8
    [(96613.76, 41015.32, 2515.00), (96613.76, 41015.31, 3000.00)],  # S11
    [(98589.15, 39483.05, 2515.00), (98589.14, 39483.04, 3000.00)],  # S14
    # Pipe 3 connections
    [(92219.87, 47587.50, 2522.50), (92219.93, 47587.58, 3000.00)],  # S3
    [(94195.25, 46055.24, 2522.50), (94195.30, 46055.30, 3000.00)],  # S6
    [(96170.64, 44522.97, 2522.50), (96170.68, 44523.01, 3000.00)],  # S9
    [(98146.03, 42990.70, 2522.50), (98146.05, 42990.73, 3000.00)],  # S12
    [(100121.42, 41458.43, 2522.50), (100121.42, 41458.44, 3000.00)], # S15
]

# ===============================
# CREATE FIGURE
# ===============================

fig = go.Figure()

# 🔹 Room Boundary
fig.add_trace(go.Scatter3d(
    x=room_x,
    y=room_y,
    z=room_z,
    mode='lines',
    line=dict(color='black', width=6),
    name='Room Boundary'
))

# Pipes (colour-coded)
pipe_colors = ['royalblue', 'darkorange', 'green']
for i, pipe in enumerate(pipes):
    fig.add_trace(go.Scatter3d(
        x=[pipe[0][0], pipe[1][0]],
        y=[pipe[0][1], pipe[1][1]],
        z=[pipe[0][2], pipe[1][2]],
        mode='lines',
        line=dict(color=pipe_colors[i], width=8),
        name=f'Pipe {i+1}'
    ))

# Sprinklers
pipe_marker_colors = ['royalblue', 'royalblue', 'royalblue', 'royalblue', 'royalblue',
                      'darkorange', 'darkorange', 'darkorange', 'darkorange', 'darkorange',
                      'green', 'green', 'green', 'green', 'green']

fig.add_trace(go.Scatter3d(
    x=[s[0] for s in sprinklers],
    y=[s[1] for s in sprinklers],
    z=[s[2] for s in sprinklers],
    mode='markers+text',
    marker=dict(size=6, color=pipe_marker_colors, symbol='circle'),
    text=sprinkler_labels,
    textposition="top center",
    name='Sprinklers'
))

# 🔹 Connections
conn_colors = (
    ['royalblue'] * 5 +
    ['darkorange'] * 5 +
    ['green'] * 5
)
for i, conn in enumerate(connections):
    fig.add_trace(go.Scatter3d(
        x=[conn[0][0], conn[1][0]],
        y=[conn[0][1], conn[1][1]],
        z=[conn[0][2], conn[1][2]],
        mode='lines',
        line=dict(color=conn_colors[i], width=3, dash='dash'),
        showlegend=(i == 0),  # Only show one legend entry for connections
        name='Connection' if i == 0 else None,
        legendgroup='connections'
    ))

# ===============================
# LAYOUT SETTINGS
# ===============================

fig.update_layout(
    title="3D Fire Sprinkler Layout",
    scene=dict(
        xaxis_title="X (mm)",
        yaxis_title="Y (mm)",
        zaxis_title="Z (mm)",
        aspectmode='data'  # Maintains true engineering proportions
    ),
    legend=dict(x=0.02, y=0.98)
)

fig.show()
fig.write_html("sprinkler_layout.html")
