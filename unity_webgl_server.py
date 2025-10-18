#!/usr/bin/env python3
"""
Servidor web simple para servir builds de Unity WebGL
Accesible desde otros dispositivos en la misma red
"""
import http.server
import socketserver
import socket
import os
import sys

def get_local_ip():
    """Obtiene la IP local de la máquina"""
    try:
        # Crear un socket temporal para obtener la IP local
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        local_ip = s.getsockname()[0]
        s.close()
        return local_ip
    except:
        return "127.0.0.1"

class UnityWebGLHandler(http.server.SimpleHTTPRequestHandler):
    """Handler personalizado para servir archivos Unity WebGL con headers correctos"""
    
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
    
    def end_headers(self):
        # Añadir headers necesarios para Unity WebGL
        self.send_header('Cross-Origin-Embedder-Policy', 'require-corp')
        self.send_header('Cross-Origin-Opener-Policy', 'same-origin')
        super().end_headers()
    
    def guess_type(self, path):
        """Determina el tipo MIME correcto para archivos Unity"""
        mimetype, encoding = super().guess_type(path)
        
        # Headers específicos para archivos Unity WebGL
        if path.endswith('.wasm'):
            return 'application/wasm'
        elif path.endswith('.data'):
            return 'application/octet-stream'
        elif path.endswith('.js'):
            return 'application/javascript'
        elif path.endswith('.symbols.json'):
            return 'application/json'
        
        return mimetype

def main():
    # Configuración del servidor
    PORT = 8000
    
    if len(sys.argv) > 1:
        try:
            PORT = int(sys.argv[1])
        except ValueError:
            print("Puerto inválido, usando puerto 8000")
    
    # Obtener IP local
    local_ip = get_local_ip()
    
    # Crear el servidor
    with socketserver.TCPServer(("0.0.0.0", PORT), UnityWebGLHandler) as httpd:
        print("=" * 60)
        print(f"🚀 Servidor Unity WebGL iniciado!")
        print("=" * 60)
        print(f"📱 Acceso local:     http://localhost:{PORT}")
        print(f"🌐 Acceso en red:    http://{local_ip}:{PORT}")
        print("=" * 60)
        print(f"📁 Sirviendo desde:  {os.getcwd()}")
        print("🛑 Presiona Ctrl+C para detener el servidor")
        print("=" * 60)
        
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\n🛑 Servidor detenido.")

if __name__ == "__main__":
    main()