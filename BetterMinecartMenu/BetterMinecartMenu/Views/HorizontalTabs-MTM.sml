<frame layout="content content">
    <lane layout="content content" orientation="vertical">
        <lane layout="1075px content">
            <banner layout="350px content"
                    background={@Mods/StardewUI/Sprites/BannerBackground}
                    background-border-thickness="48,0"
                    padding="80, 12"
                    margin="250, 0, 0, 20"
                    text={NetworkName} />
            <spacer layout="stretch" />
        </lane>
        <lane orientation="vertical" button-press=|HandleButtonPress($Button)|>
            <lane orientation="horizontal" margin="32, 0, 0, -12" z-index="1">
                <tab *repeat={:Tabs}
                    layout="92px"
                    tooltip={:Tooltip}
                    active={<>Active}
                    activate=|Clicked()|>
                    <label text={:Name} />
                </tab>
            </lane>
            <frame layout="1000px 400px"
                   background={@Mods/StardewUI/Sprites/MenuBackground}
                   border={@Mods/StardewUI/Sprites/MenuBorder}
                   border-thickness="36, 36, 40, 36"
                   padding="8">
                <scrollable peeking="128">
                    <grid layout="stretch content"
                          item-layout="count: 3"
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
                                        <label text={:Name} horizontal-alignment="middle" max-lines="1" font={^LocalisedFont} bold={^LocalisedFontBold} scale={^LocalisedFontScale} />
                                    </panel>
                                </panel>

                                <panel layout="stretch" vertical-content-alignment="end" opacity={:StatusOpacity}>
                                    <frame
                                        layout="stretch 20px"
                                        opacity="0.8"
                                        background={@Mods/StardewUI/Sprites/MenuBackground} />
                                    <panel
                                        layout="stretch 15px"
                                        horizontal-content-alignment="middle"
                                        vertical-content-alignment="middle">
                                        <label text={:StatusText} horizontal-alignment="middle" max-lines="1" font={^LocalisedFont} bold={^LocalisedFontBold} scale={^LocalisedFontScale} />
                                    </panel>
                                </panel>
                            </panel>
                        </panel>
                    </grid>
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
