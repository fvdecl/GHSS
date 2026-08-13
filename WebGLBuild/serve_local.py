#!/usr/bin/env python3
"""Serves this Unity WebGL build over HTTP and opens it in the default browser.

WebGL builds must be served via http:// - opening index.html directly with
file:// fails because browsers block fetching the .wasm/.data files under
that protocol (CORS/MIME restrictions). This is a plain local static server,
stdlib only, nothing to install.

Usage:
    python serve_local.py [port]      (default port: 8000)
"""
import http.server
import mimetypes
import socket
import sys
import threading
import webbrowser
from pathlib import Path

DEFAULT_PORT = 8000
BUILD_DIR = Path(__file__).resolve().parent

# WebGL needs the correct MIME type for the browser to instantiate the
# WebAssembly module; not every Python install has this registered by default.
mimetypes.add_type("application/wasm", ".wasm")


class Handler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(BUILD_DIR), **kwargs)


def find_free_port(preferred: int) -> int:
    for port in range(preferred, preferred + 20):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            if sock.connect_ex(("127.0.0.1", port)) != 0:
                return port
    raise RuntimeError(f"No free port found near {preferred}")


def main():
    requested_port = int(sys.argv[1]) if len(sys.argv) > 1 else DEFAULT_PORT
    port = find_free_port(requested_port)
    url = f"http://127.0.0.1:{port}/"

    with http.server.ThreadingHTTPServer(("127.0.0.1", port), Handler) as httpd:
        print(f"Serving {BUILD_DIR}")
        print(f"Open: {url}")
        print("Press Ctrl+C to stop.")

        threading.Timer(0.5, lambda: webbrowser.open(url)).start()

        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\nStopped.")


if __name__ == "__main__":
    main()
