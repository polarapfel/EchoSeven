.PHONY: clean all install restore uninstall publish build post-install default

default: publish

restore:
	cd EchoSevenUtility && dotnet restore
	cd EchoSeven && dotnet restore

clean:
	cd EchoSeven && dotnet clean
	cd EchoSevenUtility && dotnet clean
	if [ -d "./target" ]; then rm -rf ./target; fi

publish: restore
	mkdir -p ./target/usr/share/man/man8
	mkdir -p ./target/opt/EchoSeven
	mkdir -p ./target/etc/EchoSeven
	mkdir -p ./target/etc/systemd/system/
	chmod -R 755 ./target/
	cd ./EchoSeven && dotnet publish -c Release -o ../target/opt/EchoSeven
	cp ./packagefiles/EchoSeven.service ./target/etc/systemd/system/EchoSeven.service
	cp ./packagefiles/appsettings.json ./target/etc/EchoSeven/appsettings.json
	cp ./packagefiles/echoseven ./target/usr/share/man/man8/echoseven.1
	chmod 0644 ./target/usr/share/man/man8/echoseven.1
	gzip ./target/usr/share/man/man8/echoseven.1

build: publish

all: clean restore publish

install:
ifdef $(DESTDIR)
	mkdir -p $(DESTDIR)
endif
	cp -u -r -v ./target/* $(DESTDIR)/
	chmod 600 $(DESTDIR)/etc/EchoSeven/appsettings.json

post-install:
	useradd -r -s /sbin/nologin echoseven
	systemctl daemon-reload

uninstall:
	rm -rf $(DESTDIR)/opt/EchoSeven
	rm $(DESTDIR)/etc/systemd/system/EchoSeven.service
	rm -rf $(DESTDIR)/etc/EchoSeven
	rm $(DESTDIR)/usr/share/man/man8/echoseven.1.gz

post-uninstall:
	userdel echoseven
	systemctl daemon-reload
