#!/usr/bin/env bash
# irooon リリーススクリプト
# プロジェクトルート/Release ディレクトリに単一の実行可能ファイルを出力

set -euo pipefail

VERSION="${1:-0.7.0}"
BUILD_ALL=false
BUILD_WINDOWS=false
BUILD_LINUX=false
BUILD_MAC=false

# 引数をパース
while [[ $# -gt 0 ]]; do
    case $1 in
        --all)
            BUILD_ALL=true
            shift
            ;;
        --windows)
            BUILD_WINDOWS=true
            shift
            ;;
        --linux)
            BUILD_LINUX=true
            shift
            ;;
        --mac)
            BUILD_MAC=true
            shift
            ;;
        --version)
            VERSION="$2"
            shift 2
            ;;
        *)
            VERSION="$1"
            shift
            ;;
    esac
done

# デフォルト: オプション指定なしの場合は現在のプラットフォームのみビルド
if [[ "$BUILD_ALL" == false && "$BUILD_WINDOWS" == false && "$BUILD_LINUX" == false && "$BUILD_MAC" == false ]]; then
    case "$(uname -s)" in
        Linux*)
            BUILD_LINUX=true
            ;;
        Darwin*)
            BUILD_MAC=true
            ;;
        MINGW*|MSYS*|CYGWIN*)
            BUILD_WINDOWS=true
            ;;
        *)
            echo "Unknown platform: $(uname -s)"
            exit 1
            ;;
    esac
fi

# プロジェクトルートに移動
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

echo "=== irooon Release Build ==="
echo "Version: $VERSION"
echo ""

# Releaseディレクトリを作成
RELEASE_DIR="$PROJECT_ROOT/Release"
if [[ -d "$RELEASE_DIR" ]]; then
    echo "Cleaning Release directory..."
    rm -rf "$RELEASE_DIR"
fi
mkdir -p "$RELEASE_DIR"

# ビルド関数
build_platform() {
    local runtime="$1"
    local platform_name="$2"

    echo "Building for $platform_name ($runtime)..."

    local output_dir="$RELEASE_DIR/$platform_name"

    # dotnet publish で単一ファイル実行可能ファイルを生成
    dotnet publish src/Irooon.Cli/Irooon.Cli.csproj \
        --configuration Release \
        --runtime "$runtime" \
        --self-contained true \
        --output "$output_dir" \
        -p:PublishSingleFile=true \
        -p:IncludeNativeLibrariesForSelfExtract=true \
        -p:EnableCompressionInSingleFile=true \
        -p:Version="$VERSION"

    if [[ $? -eq 0 ]]; then
        echo "✓ Build completed: $output_dir"

        # 実行可能ファイルのパスを表示
        local exe_name
        if [[ "$runtime" == win-* ]]; then
            exe_name="Irooon.Cli.exe"
        else
            exe_name="Irooon.Cli"
        fi

        local exe_path="$output_dir/$exe_name"
        if [[ -f "$exe_path" ]]; then
            local file_size=$(du -h "$exe_path" | cut -f1)
            echo "  Executable: $exe_path"
            echo "  Size: $file_size"
        fi
        echo ""
    else
        echo "✗ Build failed for $platform_name"
        exit 1
    fi
}

# プラットフォームごとにビルド
if [[ "$BUILD_ALL" == true || "$BUILD_WINDOWS" == true ]]; then
    build_platform "win-x64" "windows-x64"
fi

if [[ "$BUILD_ALL" == true || "$BUILD_LINUX" == true ]]; then
    build_platform "linux-x64" "linux-x64"
fi

if [[ "$BUILD_ALL" == true || "$BUILD_MAC" == true ]]; then
    build_platform "osx-x64" "macos-x64"
    build_platform "osx-arm64" "macos-arm64"
fi

echo "=== Release Build Complete ==="
echo "Output directory: $RELEASE_DIR"
echo ""
echo "Usage:"
echo "  ./scripts/release.sh                # 現在のプラットフォームのみ"
echo "  ./scripts/release.sh --all          # すべてのプラットフォーム"
echo "  ./scripts/release.sh --windows      # Windows のみ"
echo "  ./scripts/release.sh --linux        # Linux のみ"
echo "  ./scripts/release.sh --mac          # macOS のみ"
echo "  ./scripts/release.sh --version 0.7.0  # バージョン指定"
