<frame layout="content content">
    <lane layout="content content" orientation="vertical">
        <lane layout="1075px content">
            <banner layout="stretch content"
                    background={@Mods/StardewUI/Sprites/BannerBackground}
                    background-border-thickness="48,0"
                    padding="80, 12"
                    margin="450, 0, 120, 0"
                    text={NetworkName} />
        </lane>
        <lane orientation="horizontal" button-press=|HandleButtonPress($Button)|>
            <lane layout="335px content" orientation="vertical" margin="0, 30, 0, 0">
                <frame *repeat={:Tabs}
                    layout="stretch 64px"
                    margin="0, 3"
                    padding="0, 0, 40, 0"
                    horizontal-content-alignment="middle"
                    vertical-content-alignment="middle"
                    background={@Mods/StardewUI/Sprites/ControlBorder}
                    transform={Transform}
                    focusable="true"
                    click=|Clicked()|>
                    <label text={:Tooltip} />
                </frame>
            </lane>
            <frame layout="665px 700px"
                   background={@Mods/StardewUI/Sprites/MenuBackground}
                   border={@Mods/StardewUI/Sprites/MenuBorder}
                   border-thickness="36, 36, 40, 36"
                   padding="8">
                <scrollable peeking="128">
                    <lane layout="stretch content" orientation="vertical">
                        <grid layout="stretch content"
                              item-layout="count: 2"
                              item-spacing="16,16"
                              horizontal-item-alignment="middle">
                            <button *repeat={:Destinations}
                                layout="stretch 100px"
                                Opacity={:Opacity}
                                click=|Clicked()|>
                                <label text={:Name} />
                            </button>
                        </grid>
                    </lane>
                </scrollable>
            </frame>
        </lane>
        <lane layout="1075px content">
            <spacer layout="stretch" />
            <image layout="70px 70px"
                   horizontal-alignment="middle"
                   vertical-alignment="middle"
                   focusable="true"
                   click=|ClickedClose()|
                   sprite={@Mods/StardewUI/Sprites/CloseButton} />
        </lane>
    </lane>
</frame>
