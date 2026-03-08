SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

(
  cd "$SCRIPT_DIR/.."
  git submodule update --init --recursive;
  bash $SCRIPT_DIR/build-yoga.sh;
)