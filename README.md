# kartrider-parts-crawler

[![standard-readme compliant](https://img.shields.io/badge/standard--readme-OK-green.svg?style=flat-square)](https://github.com/RichardLitt/standard-readme)

카트라이더 웹 차고에서 파츠를 크롤링합니다.

## Table of Contents

- [Install](#install)
- [Usage](#usage)
- [Maintainers](#maintainers)
- [Contributing](#contributing)
- [License](#license)

## Install
크롬 87버전이 필요합니다.  
그 외에 버전을 사용하실려면 `Selenium.WebDriver.ChromeDriver`의 다른 버전을 설치하세요.
```bash
git clone https://github.com/zxc010613/kartrider-parts-crawler.git
cd kartrider-parts-crawler
dotnet restore
```

## Usage

```bash
cd "Kartrider Parts Crawler"
dotnet build -c Relase
dotnet run '.\Kartrider Parts Crawler.csproj' -c Release
```

## Maintainers

[@zxc010613](https://github.com/zxc010613)

## Contributing
PR 허용

## License
[MIT LICENSE](./LICENSE)
