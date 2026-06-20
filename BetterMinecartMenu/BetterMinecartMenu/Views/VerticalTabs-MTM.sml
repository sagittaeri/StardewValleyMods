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
            <lane layout="235px content" orientation="vertical" margin="0, 30, 0, 0">
                <frame *repeat={:Tabs}
                    layout="stretch 56px"
                    margin="0, 3"
                    padding="0, 0, 40, 0"
                    horizontal-content-alignment="start"
                    vertical-content-alignment="middle"
                    background={@Mods/StardewUI/Sprites/ControlBorder}
                    transform={Transform}
                    focusable="true"
                    click=|Clicked()|>
                    <panel margin="60,0,0,0">
                        <label text={:Tooltip} horizontal-alignment="middle" font="tiny" margin="0,0,0,0" scale="0.5" />
                        <label text={:Tooltip} horizontal-alignment="middle" font="tiny" margin="1,0,0,0" scale="0.5" />
                        <label text={:Tooltip} horizontal-alignment="middle" font="tiny" margin="0,1,0,0" scale="0.5" />
                    </panel>
                </frame>
            </lane>
            <frame layout="665px 660px"
                   background={@Mods/StardewUI/Sprites/MenuBackground}
                   border={@Mods/StardewUI/Sprites/MenuBorder}
                   border-thickness="36, 36, 40, 36"
                   padding="8">
                <scrollable peeking="128">
                    <lane layout="stretch content" orientation="vertical">
                        <grid layout="stretch content"
                              item-layout="count: 2"
                              item-spacing="10,10"
                              horizontal-item-alignment="middle">
                            <panel *repeat={:Destinations}
                                layout="stretch content">
                                <button
                                    layout="stretch"
                                    opacity={:ButtonOpacity}
                                    click=|Clicked()|>
                                </button>
                                <panel
                                    padding="12,12">
                                    <panel opacity={:TextureOpacity}>
                                        <image
                                            layout="stretch content"
                                            sprite={DestTexture} />
                                    </panel>
                                    <panel layout="stretch" vertical-content-alignment="start" margin="0,-1,0,0">
                                        <frame
                                            layout="stretch 40px"
                                            opacity="0.8"
                                            background={@Mods/StardewUI/Sprites/MenuBackground} />
                                        <panel
                                            layout="stretch 40px"
                                            horizontal-content-alignment="middle"
                                            vertical-content-alignment="middle">
                                            <label text={:Name} horizontal-alignment="middle" font="tiny" margin="0,0,0,0" scale="0.5" />
                                            <label text={:Name} horizontal-alignment="middle" font="tiny" margin="1,1,0,0" scale="0.5" />
                                            <label text={:Name} horizontal-alignment="middle" font="tiny" margin="-1,-1,0,0" scale="0.5" />
                                            <label text={:Name} horizontal-alignment="middle" font="tiny" margin="1,-1,0,0" scale="0.5" />
                                            <label text={:Name} horizontal-alignment="middle" font="tiny" margin="-1,1,0,0" scale="0.5" />
                                        </panel>
                                    </panel>

                                    <panel layout="stretch" vertical-content-alignment="end" opacity={:StatusOpacity}>
                                        <frame
                                            layout="stretch 40px"
                                            opacity="0.8"
                                            background={@Mods/StardewUI/Sprites/MenuBackground} />
                                        <panel
                                            layout="stretch 35px"
                                            horizontal-content-alignment="middle"
                                            vertical-content-alignment="middle">
                                            <label text={:StatusText} horizontal-alignment="middle" font="tiny" margin="1,1,0,0" scale="0.5" />
                                            <label text={:StatusText} horizontal-alignment="middle" font="tiny" margin="-1,-1,0,0" scale="0.5" />
                                            <label text={:StatusText} horizontal-alignment="middle" font="tiny" margin="1,-1,0,0" scale="0.5" />
                                            <label text={:StatusText} horizontal-alignment="middle" font="tiny" margin="-1,1,0,0" scale="0.5" />
                                        </panel>
                                    </panel>
                                </panel>
                            </panel>
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
