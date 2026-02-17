import plotly.graph_objects as go

# ===============================
# ROOM CORNERS
# ===============================

room_x = [97500.01, 85647.67, 91776.75, 103629.07,97500.01]
room_y = [34000.00, 43193.61, 51095.16, 41901.55,34000.00]
room_z = [2500.00, 2500.00, 2530.00, 2530.00,2500.00]

# ===============================
# WATER PIPES
# ===============================

pipes = [
    [(98242.11, 36588.29, 3000.00), (87970.10, 44556.09, 3000.00)],
    [(99774.38, 38563.68, 3000.00), (89502.37, 46531.47, 3000.00)],
    [(101306.65, 40539.07, 3000.00), (91034.63, 48507.01, 3000.00)],
]

# ===============================
# SPRINKLERS
# ===============================

sprinklers = [
    (91655.33, 42507.66, 2515.00),
    (91655.33, 45007.66, 2515.00),
    (94155.33, 40007.66, 2515.00),
    (94155.33, 42507.66, 2515.00),
    (94155.33, 45007.66, 2515.00),
    (96655.33, 40007.66, 2515.00),
    (96655.33, 42507.66, 2515.00),
]

# ===============================
# CONNECTIONS (Sprinkler -> Pipe Point)
# ===============================

connections = [
    [(91655.33, 42507.66, 2515.00), (91262.99, 42001.86, 3000.00)],
    [(91655.33, 45007.66, 2515.00), (91584.53, 44916.38, 3000.00)],
    [(94155.33, 40007.66, 2515.00), (94034.58, 39851.99, 3000.00)],
    [(94155.33, 42507.66, 2515.00), (94356.12, 42766.52, 3000.00)],
    [(94155.33, 45007.66, 2515.00), (94677.71, 45681.10, 3000.00)],
    [(96655.33, 40007.66, 2515.00), (97127.71, 40616.65, 3000.00)],
    [(96655.33, 42507.66, 2515.00), (95916.98, 41555.79, 3000.00)],
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

# 🔹 Pipes
for i, pipe in enumerate(pipes):
    fig.add_trace(go.Scatter3d(
        x=[pipe[0][0], pipe[1][0]],
        y=[pipe[0][1], pipe[1][1]],
        z=[pipe[0][2], pipe[1][2]],
        mode='lines',
        line=dict(width=8),
        name=f'Pipe {i+1}'
    ))

# 🔹 Sprinklers
fig.add_trace(go.Scatter3d(
    x=[s[0] for s in sprinklers],
    y=[s[1] for s in sprinklers],
    z=[s[2] for s in sprinklers],
    mode='markers+text',
    marker=dict(size=6),
    text=[f"S{i+1}" for i in range(len(sprinklers))],
    textposition="top center",
    name='Sprinklers'
))

# 🔹 Connections
for i, conn in enumerate(connections):
    fig.add_trace(go.Scatter3d(
        x=[conn[0][0], conn[1][0]],
        y=[conn[0][1], conn[1][1]],
        z=[conn[0][2], conn[1][2]],
        mode='lines',
        line=dict(width=4, dash='dash'),
        name=f'Connection {i+1}'
    ))

# ===============================
# LAYOUT SETTINGS (VERY IMPORTANT)
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
