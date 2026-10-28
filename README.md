Nombre del juego: CardPush

Hecho por Gabriel Rivas

Pipeline: Universal Render Pipeline (URP) para manejar luces 2D y materiales Sprites-Default (sirvio tambien para solucionar bug de animación).

Físicas: Rigidbody2D e impulsos físicos para el sistema de empuje al perder turnos.

Audio: se utilizo ControladorAudio en patrón Singleton con DontDestroyOnLoad para mantener la música entre escenas.

Persistencia: se utilizo PlayerPrefs para guardar las victorias en Modo Ronda y desbloquear el Modo Pintas (se necesita ganar 3 veces en modo ronda).

## Riesgos y Mitigaciones

| Riesgo | Mitigación |
| --- | --- |
| Personaje se vuelve invisible por luz o capas | Se usó material Unlit (Sprites-Default) y se subió el Order in Layer. |
| Audio duplicado al cambiar de escena | Se usó Singleton con DontDestroyOnLoad para destruir duplicados. |
| Pérdida de progreso al cerrar el juego | Se usa PlayerPrefs.Save() inmediatamente al registrar la victoria. |
