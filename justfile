set shell := ["bash", "-eu", "-o", "pipefail", "-c"]

solution := "Kataka.slnx"
domain_project := "src/Kataka.Domain/Kataka.Domain.csproj"
infra_project := "src/Kataka.Infrastructure/Kataka.Infrastructure.csproj"
app_project := "src/Kataka.App/Kataka.App.csproj"
test_project := "tests/Kataka.Tests/Kataka.Tests.csproj"
local_nuget_dir := "../../LocalNuget"

build:
    dotnet build {{solution}} -c Release

run:
    dotnet run --project {{app_project}}

test:
    dotnet test {{test_project}} -c Release

pack:
    mkdir -p artifacts/packages
    dotnet pack {{domain_project}} -c Release -o artifacts/packages
    dotnet pack {{infra_project}} -c Release -o artifacts/packages

publish-localnuget:
    @bash -eu -o pipefail -c '\
        local_feed="{{local_nuget_dir}}"; \
        temp_dir=$$(mktemp -d); \
        trap '\''rm -rf "$$temp_dir"'\'' EXIT; \
        mkdir -p "$$local_feed" "$$temp_dir/base"; \
        dotnet pack "{{domain_project}}" -c Release -o "$$temp_dir/base" >/dev/null; \
        dotnet pack "{{infra_project}}" -c Release -o "$$temp_dir/base" >/dev/null; \
        python - "$$temp_dir/base" "$$local_feed" <<'\''PY'\'' \
from pathlib import Path \
import sys \
import zipfile \
import xml.etree.ElementTree as ET \
 \
base_dir = Path(sys.argv[1]) \
local_feed = Path(sys.argv[2]) \
 \
for package in sorted(base_dir.glob("*.nupkg")): \
    if package.name.endswith(".symbols.nupkg"): \
        continue \
 \
    with zipfile.ZipFile(package, "r") as existing: \
        nuspec_name = next((name for name in existing.namelist() if name.endswith(".nuspec")), None) \
        if nuspec_name is None: \
            raise SystemExit(f"Could not locate .nuspec in {package.name}") \
        root = ET.fromstring(existing.read(nuspec_name)) \
        namespace = {"n": root.tag.split("}")[0].strip("{")} if root.tag.startswith("{") else {} \
        id_node = root.find("n:metadata/n:id", namespace) if namespace else root.find("metadata/id") \
        version_node = root.find("n:metadata/n:version", namespace) if namespace else root.find("metadata/version") \
        if id_node is None or version_node is None or not id_node.text or not version_node.text: \
            raise SystemExit(f"Could not read package metadata from {package.name}") \
        package_id = id_node.text \
        base_version = version_node.text \
 \
    local_version = f"{base_version}-local" \
    output_package = local_feed / f"{package_id}.{local_version}.nupkg" \
 \
    with zipfile.ZipFile(package, "r") as existing, zipfile.ZipFile(output_package, "w", compression=zipfile.ZIP_DEFLATED) as rewritten: \
        for item in existing.infolist(): \
            data = existing.read(item.filename) \
            if item.filename.endswith(".nuspec"): \
                root = ET.fromstring(data) \
                namespace = {"n": root.tag.split("}")[0].strip("{")} if root.tag.startswith("{") else {} \
                version_node = root.find("n:metadata/n:version", namespace) if namespace else root.find("metadata/version") \
                if version_node is None: \
                    raise SystemExit(f"Could not locate <version> in {item.filename}") \
                version_node.text = local_version \
                data = ET.tostring(root, encoding="utf-8", xml_declaration=True) \
            rewritten.writestr(item, data) \
 \
    print(f"Published {output_package} ({local_version})") \
PY'
